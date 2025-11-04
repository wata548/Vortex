using System.Collections.Generic;
using System.Threading.Tasks;
using Extension;
using Extension.Test;
using UnityEngine;

namespace MapGenerator {
    
    public partial class ChunkManager: MonoSingleton<ChunkManager> {
        
        //==================================================||Constants 
        private const int SIZE = 5;
        private const int MESH_RESTORE_LIMIT = 1200;
        private static readonly Vector3 CAMERA_LOCAL_POS = new(0, 0.25f, 0);
        
        //==================================================||Properties 
        protected override bool IsNarrowSingleton { get; set; } = true;
        //==================================================||Fields 
        [SerializeField]private int _meshThreadCnt = 1;
        [SerializeField]private int _mapThreadCnt = 6;
        
        //Prefabs
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private GameObject _playerPrefab;
        private GameObject _player = null;
        
        //Generator Information
        private MapMeshGenerator _generator;
        [SerializeField] private MapGenerationArgs _args = new(pOctave:2);
        private Transform _chunkParent;
        private Vector3Int _playerChunk;
        
        //Current Chunk map
        private Chunk[,] _chunks = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];
        private Chunk[,] _temp = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];
        
        private bool _isQuit = false;
        //==================================================||Methods 

        [TestMethod]
        private void Test() {
            var pos = _chunks[SIZE, SIZE].Idx;
            for (int i = 0; i < _chunkDataStore[pos].Map.GetLength(2); i++) {
                _chunkDataStore[pos].Map[i, 0, i] = Block.Air;
            }
            _rebakeMeshPosStack.Push(pos);
        }

        public bool GetMapData(Vector3 pPos, out Block[,,] pMap) =>
            GetMapData(ToChunkIdx(pPos), out pMap);
        
        public bool GetMapData(Vector3Int pPos, out Block[,,] pMap) {
            if (!_chunkDataStore.TryGetValue(pPos, out var info)) {
                pMap = null;
                _generateMapDataStack.Push(pPos);
                return false;
            }

            pMap = info.Map;
            return true;
        }
        
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
                    _generateMapDataStack.Push(idx);
                    
                    _chunks[i + SIZE,j + SIZE] = chunk;
                }
            }
        }

        private void NewChunksLoad() {
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
                    _generateMapDataStack.Push((pos));
                }
            }

            (_chunks, _temp) = (_temp, _chunks);
        }
        
        private Vector3Int ToChunkIdx(Vector3 pPos) {
            var x = pPos.x / _args.ChunkLength;
            x = x.Sign() * (Mathf.Abs(x) + 0.5f);
            var z = pPos.z / _args.ChunkLength;
            z = z.Sign() * (Mathf.Abs(z) + 0.5f);

            return new((int)x, 0, (int)z);
        }
        
        //==================================================||Unity 
        
        private void Start() {
            
            _generator = new MapMeshGenerator();
            var chunkParent = new GameObject("Chunks");
            _chunkParent = chunkParent.transform;

            Init();
            Task.Run(GetRebakedMeshData);
#if UNITY_EDITOR
            _tasks = new Task[_meshThreadCnt + _mapThreadCnt];
            for (int i = 0; i < _mapThreadCnt; i++) {
                _tasks[i] = Task.Run(GetMapData);
            }
            for (int i = 0; i < _meshThreadCnt; i++) {
                _tasks[_mapThreadCnt + i] = Task.Run(GetMeshData);
            }
#else
            for (int i = 0; i < _mapThreadCnt; i++) {
                Task.Run(GetMapData);
            }
            for (int i = 0; i < _meshThreadCnt; i++) {
                Task.Run(GetMeshData);
            }
#endif
            
        }
        
        private void Update() {
            base.Update();
            
#if UNITY_EDITOR
            Log();
#endif
            
            if (_player == null && Input.GetKeyDown(KeyCode.U))
                SpawnPlayer();
            
            while (_rebakeMeshDataStack.Count != 0) {
                if (_rebakeMeshDataStack.TryPop(out var value)) {
                    Debug.Log($"refresh: {value.Idx}");
                    RegisterMesh(value);
                }
            }
            
            while (_meshDataStack.Count != 0) {
                if (_meshDataStack.TryPop(out var value)) {
                    Debug.Log($"Bake: {value.Idx}");
                    RegisterMesh(value);
                }
            }
            LoadAllMesh();
            
            if(_player != null)
                NewChunksLoad();
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