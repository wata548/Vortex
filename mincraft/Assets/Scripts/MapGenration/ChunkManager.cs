using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extension;
using UnityEngine;

namespace MapGenerator {
    
    public struct MeshData {

        public Vector3Int Idx;
        public Block[,,] Map;
        public List<Vector4> Uvs;
        public int[] Triangles;
        public List<Vector3> Vertices;
    }
    
    public class ChunkManager: MonoBehaviour {
        
        //==================================================||Constants 
        private const int SIZE = 3;
        private const int MESH_RESTORE_LIMIT = 1200;
        private static readonly Vector3 CAMERA_LOCAL_POS = new(0, 0.25f, 0);
        
        //==================================================||Fields 
        [SerializeField]private int _threadCnt = 4;
        
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private GameObject _playerPrefab;
        private GameObject _player = null;
        
        private MapMeshGenerator _generator;
        [SerializeField] private MapGenerationArgs _args = new(pOctave:2);
        private Transform _chunkParent;
        private Vector3Int _playerChunk;
        private Chunk[,] _chunks = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];
        private Chunk[,] _temp = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];

        private readonly ConcurrentQueue<Vector3Int> _generateMeshPosQueue = new();
        private readonly ConcurrentQueue<MeshData> _meshDataQueue = new();
        
        private readonly Dictionary<Vector3Int, (Mesh Mesh, Block[,,] Map)> _chunkMeshStore = new();
        private readonly Queue<Vector3Int> _chunkStoreHistory = new();

        private bool _isQuit = false;
        //==================================================||Methods 

        private void SpawnPlayer() {
            _player = Instantiate(_playerPrefab);
            
            var camera = Camera.main!;
            camera.transform.SetParent(_player.transform);
            camera.transform.localPosition = CAMERA_LOCAL_POS;
            
            _player.transform.position = Vector3.up * _args.ChunkHeight;
        }
        
        private void Init() {
            _playerChunk = Vector3Int.zero;
                    
            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
            for (int i = -SIZE; i <= SIZE; i++) {
                for (int j = -SIZE; j <= SIZE; j++) {
                    var chunk = Instantiate(_chunkPrefab, _chunkParent);
                    var idx = new Vector3Int(j, 0, i);
                    
                    chunk.Ready(idx);
                    _generateMeshPosQueue.Enqueue(idx);
                    
                    _chunks[i + SIZE,j + SIZE] = chunk;
                }
            }
        }
        
        private void ChunkRefresh() {
            var newChunkIdx = ToChunkIdx(_player.transform.position);
            if (newChunkIdx == _playerChunk)
                return;

            var chunkDelta = newChunkIdx - _playerChunk;
            var size = 2 * SIZE + 1;
            _playerChunk = newChunkIdx;

            bool[] visitCheck = new bool[size * size];
            var pos = -chunkDelta;
            var tempPool = new Queue<Chunk>();
            for (int i = 0; i < size; i++, pos.z++) {
                pos.x = -chunkDelta.x;
                for (int j = 0; j < size; j++, pos.x++) {

                    if (pos.x is < 0 or >= 2 * SIZE + 1 || pos.z is < 0 or >= 2 * SIZE + 1) {
                        tempPool.Enqueue(_chunks[i, j]);
                        continue;
                    }
                    visitCheck[pos.z * size + pos.x] = true;
                    _temp[pos.z, pos.x] = _chunks[i, j];
                }
            }

            newChunkIdx.x -= SIZE;
            newChunkIdx.z -= SIZE;
            
            pos = newChunkIdx;
            for (int i = 0; i < size; i++, pos.z++) {
                pos.x = newChunkIdx.x;
                for (int j = 0; j < size; j++, pos.x++) {
                    if(visitCheck[i * size + j])
                        continue;

                    var targetChunk = tempPool.Dequeue();
                    _temp[i, j] = targetChunk;
                    
                    targetChunk.Ready(pos);
                    _generateMeshPosQueue.Enqueue((pos));
                }
            }

            (_chunks, _temp) = (_temp, _chunks);
        }

        private Task GetMeshData() {
            lock (_temp) {
                Thread.Sleep(3);
            }
            while (true) {
                if (_isQuit)
                    return null;
                
                if(!_generateMeshPosQueue.TryDequeue(out var target))
                    continue;
                if(_chunkMeshStore.ContainsKey(target))
                    continue;

                _chunkMeshStore.Add(target, (null, null));
                _meshDataQueue.Enqueue(_generator.Generate(_args, target));
                Thread.Sleep(1);
            }
        }

        private void RegisterMesh(MeshData pData) {
            var mesh = new Mesh();
            mesh.SetVertices(pData.Vertices);
            mesh.triangles = pData.Triangles;
            mesh.SetUVs(0, pData.Uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            _chunkStoreHistory.Enqueue(pData.Idx);
            _chunkMeshStore[pData.Idx] = (mesh, pData.Map);
        }

        private void LoadAllMesh() {
            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
                    
            foreach (var chunk in _chunks) {
                var idx = chunk.Idx;
                var pos = idx.Multiple(interval);
                pos -= new Vector3(_args.ChunkLength / 2f, 0, _args.ChunkLength / 2f);
        
                if(_chunkMeshStore.TryGetValue(idx, out var value))
                    chunk.SetUp(value.Mesh, pos);
            }    
        }
        
        private Vector3Int ToChunkIdx(Vector3 pPos) {
            var x = pPos.x / _args.ChunkLength;
            x = x.Sign() * (Mathf.Abs(x) + 0.5f);
            var z = pPos.z / _args.ChunkLength;
            z = z.Sign() * (Mathf.Abs(z) + 0.5f);

            return new((int)x, 0, (int)z);
        }
        
        //==================================================||Unity 
        
        private void Awake() {
            
            _generator = new MapMeshGenerator();
            var chunkParent = new GameObject("Chunks");
            _chunkParent = chunkParent.transform;

            Init();
#if UNITY_EDITOR
            _tasks = new Task[_threadCnt];
            for (int i = 0; i < _threadCnt; i++) {
                _tasks[i] = Task.Run(GetMeshData);
            }
#else
            for (int i = 0; i < _threadCnt; i++) {
                Task.Run(GetMeshData);
            }
#endif
            
        }
        
        private void Update() {

#if UNITY_EDITOR
            Log();
#endif
            
            if (_player == null && _generateMeshPosQueue.Count == 0)
                SpawnPlayer();
            
            while (_meshDataQueue.Count != 0) {
                if (_meshDataQueue.TryDequeue(out var value)) {
                    Debug.Log($"Bake: {value.Idx}");
                    RegisterMesh(value);
                }
            }
            LoadAllMesh();
            
            if(_player != null)
                ChunkRefresh();
        }
        
        private void OnApplicationQuit() {
            _isQuit = true;
        }
        
        #if UNITY_EDITOR
        private Task[] _tasks;

        private void Log() {
            int idx = 0;
            foreach (var task in _tasks) {
                idx++;
                if(task.Status == TaskStatus.Faulted)
                    Debug.Log($"{idx}: {task.Exception}");
            }    
        }
        
        #endif
    }
}