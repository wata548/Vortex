using System.Collections.Generic;
using Extension;
using TMPro;
using UnityEngine;

namespace MapGenerator {
    
    public class ChunkManager: MonoBehaviour {

        //==================================================||Constants 
        private const int SIZE = 1;
        private const int MESH_RESTORE_LIMIT = 1200;
        
        //==================================================||Fields 
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private GameObject _player;
        
        private MapMeshGenerator _generator;
        private MapGenerationArgs _args;
        private Transform _chunkParent;
        private Vector3Int _playerChunk;
        private Chunk[,] _chunks = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];
        private Chunk[,] _temp = new Chunk[2 * SIZE + 1, 2 * SIZE + 1];
        private readonly Queue<(Vector3, Chunk)> _pool = new((2 * SIZE + 1) * (2 * SIZE + 1));
        private readonly Dictionary<Vector3, Mesh> _chunkMeshStore = new();
        private readonly Queue<Vector3> _chunkStoreHistory = new();
        
        //==================================================||Methods 
        private void Init() {
            //Player generation
            _player.transform.position = Vector3.up * _args.ChunkHeight;
            _playerChunk = Vector3Int.zero;
            
            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
            for (int i = -SIZE; i <= SIZE; i++) {
                for (int j = -SIZE; j <= SIZE; j++) {
                    var pos = interval;
                    pos.z *= i;
                    pos.x *= j;
                    
                    var chunk = Instantiate(_chunkPrefab, _chunkParent);
                    chunk.Ready(new(j, 0, i));
                    _chunks[i + SIZE,j + SIZE] = GenerateMesh(chunk);
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
                    _pool.Enqueue((pos, targetChunk));
                }
            }

            (_chunks, _temp) = (_temp, _chunks);
        }

        private void ChunkPoolGenerator() {

            if (_pool.Count == 0)
                return;
            
            while (true) {
                var top = _pool.Dequeue();
                if (top.Item1 != top.Item2.Idx)
                    continue;

                GenerateMesh(top.Item2);
                break;
            }
        }
        
        private Chunk GenerateMesh(Chunk pChunk) {

            var idx = pChunk.Idx;
            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
            var pos = interval.Multiple(idx);
            pos -= new Vector3(_args.ChunkLength / 2f, 0, _args.ChunkLength / 2f);
            
            if (!_chunkMeshStore.TryGetValue(idx, out var mesh)) {
                var seedPos = idx * _args.ChunkRange;
                mesh = _generator.Generate(_args, seedPos);
                StoreMesh(idx, mesh);
            }

            pChunk.SetUp(mesh, pos);
            return pChunk;
        }

        private void StoreMesh(Vector3 pIdx, Mesh pMesh) {
            _chunkMeshStore.Add(pIdx, pMesh);
            _chunkStoreHistory.Enqueue(pIdx);
            while (_chunkStoreHistory.Count > MESH_RESTORE_LIMIT) {
                _chunkMeshStore.Remove(_chunkStoreHistory.Dequeue());
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
            _args = new MapGenerationArgs(pOctave: 2);
            var chunkParent = new GameObject("Chunks");
            _chunkParent = chunkParent.transform;
            Init();
        }
        
        private void Update() {
            ChunkRefresh();
            ChunkPoolGenerator();
        }
    }
}