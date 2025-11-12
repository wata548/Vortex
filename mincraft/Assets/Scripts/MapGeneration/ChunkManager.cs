using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Enemy;
using Extension;
using Extension.Test;
using UnityEngine;
using UnityEngine.Serialization;

namespace MapGenerator {
    
    public partial class ChunkManager: MonoSingleton<ChunkManager> {
        
        //==================================================||Constants 
        private const int SIZE = 1;
        private const int MESH_RESTORE_LIMIT = 1200;
        private static readonly Vector3 CAMERA_LOCAL_POS = new(0, 0.25f, 0);
        
        //==================================================||Properties 
        protected override bool IsNarrowSingleton { get; set; } = true;
        //==================================================||Fields 
        [SerializeField]private int _meshThreadCnt = 1;
        [SerializeField]private int _mapThreadCnt = 6;
        
        //Prefabs
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private Player.Player _playerPrefab;
        public Player.Player Player { get; private set; } = null;
        
        //Generator Information
        private MapMeshGenerator _generator;
        [field: SerializeField] public MapGenerationArgs Args { get; private set; } = new(pOctave:2);
        private Transform _chunkParent;
        private Vector3Int _playerChunk = Vector3Int.zero;
        private Vector3 _playerChunkPos = Vector3.zero;
        
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
            _reBakeMeshPosStack.Push(pos);
        }

        //return changed chunk idx
        private List<Vector3Int> SetBlock(Vector3Int pChunk, Vector3Int pPos, Block pTarget) {
            var targetChunks = new List<Vector3Int>();
            targetChunks.Add(pChunk);
            
            Debug.Log($"{pChunk} - {pPos}");
            _chunkDataStore[pChunk].Map[pPos.x, pPos.y, pPos.z] = pTarget;
            if (pPos.x == 0) {
                var newPos = pChunk;
                newPos.x--;
                _chunkDataStore[newPos].Map[Args.ChunkLength, pPos.y, pPos.z] = pTarget;
                targetChunks.Add(newPos);
            }
            if (pPos.z == 0) {
                var newPos = pChunk;
                newPos.z--;
                _chunkDataStore[newPos].Map[pPos.x, pPos.y, Args.ChunkLength] = pTarget;
                targetChunks.Add(newPos);
            }
            return targetChunks;
        }
        
        public void UpdateBlock(params (Vector3 Pos, Block Block)[] pNewDatas) {
            var targetChunks = new HashSet<Vector3Int>();
            foreach (var data in pNewDatas) {
                var pos = Chunk.GetChunkPos(Args, data.Pos, out var chunkIdx);
                foreach(var targetChunkIdx in SetBlock(chunkIdx, pos, data.Block))
                    targetChunks.Add(targetChunkIdx);
            }

            foreach (var chunk in targetChunks) {
                _reBakeMeshPosStack.Push(chunk);
            }
        }

        public bool IsLoadedChunk(Vector3Int pPos) {
            var idx = Chunk.GetChunkIdx(Args, pPos);
            return
                idx.x <= _playerChunk.x + SIZE
                && idx.x >= _playerChunk.x - SIZE
                && idx.y <= _playerChunk.y + SIZE
                && idx.y >= _playerChunk.y - SIZE;
        }
        
        public Block GetMapData(Vector3Int pPos) {

            var chunk = Chunk.GetChunkIdx(Args, pPos);
            var chunkPos = Chunk.GetChunkLocalPos(Args, pPos).ToVec3Int();
            
            if (!_chunkDataStore.TryGetValue(chunk, out var chunkData))
                return Block.Dirty;
            
            return chunkData.Map[chunkPos.x, chunkPos.y, chunkPos.z];
        }
        
        private void SpawnPlayer() {
            Player = Instantiate(_playerPrefab);
            
            var camera = Camera.main!;
            camera.transform.SetParent(Player.transform);
            camera.transform.localPosition = CAMERA_LOCAL_POS;
            
            Player.transform.position = Vector3.up * Args.ChunkHeight;
        }
        
        private void Init() {
            _playerChunk = Vector3Int.zero;
                    
            var interval = new Vector3(Args.ChunkLength, 0, Args.ChunkLength);
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
            var newChunkIdx = Chunk.GetChunkIdx(Args, Player.transform.position);
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
                        
                        EnemyManager.UnLoad(new(_playerChunk.x + pos.x - SIZE, 0, _playerChunk.z + pos.z - SIZE));
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
                    EnemyManager.Load(pos);
                    _temp[i, j] = targetChunk;
                    
                    targetChunk.Ready(pos);
                    _generateMapDataStack.Push((pos));
                }
            }

            (_chunks, _temp) = (_temp, _chunks);
        }
        
        //==================================================||Unity 
        
        private void Start() {
            
            _generator = new MapMeshGenerator();
            var chunkParent = new GameObject("Chunks");
            _chunkParent = chunkParent.transform;

            Init();
            Task.Run(GetReBakedMeshData);
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

        private int _bakeCnt = 0; 
        private void Update() {
            base.Update();
            
#if UNITY_EDITOR
            Log();
#endif
            
            if (Player == null && _bakeCnt >= (2 * SIZE + 1) * (2 * SIZE + 1))
                SpawnPlayer();
            
            while (_reBakeMeshDataStack.Count != 0) {
                if (_reBakeMeshDataStack.TryPop(out var value)) {
                    Debug.Log($"refresh: {value.Idx}");
                    RegisterMesh(value);
                }
            }
            
            while (_meshDataStack.Count != 0) {
                if (_meshDataStack.TryPop(out var value)) {
                    Debug.Log($"Bake: {value.Idx}");
                    RegisterMesh(value);
                    _bakeCnt++;
                }
            }
            LoadAllMesh();

            if (Player != null) {
                NewChunksLoad();
                _playerChunkPos = Chunk.GetChunkLocalPos(Args, Player.transform.position);
            }
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