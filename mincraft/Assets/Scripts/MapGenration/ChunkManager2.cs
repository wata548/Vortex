using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extension;
using UnityEngine;

namespace MapGenerator {
    
    public struct MeshData {
        public Block[,,] Map;
        public IEnumerable<Vector4> Uvs;
        public IEnumerable<int> Triangles;
        public IEnumerable<Vector3> Vertices;
    }
    
    public class ChunkManager2: MonoBehaviour {
        
        //==================================================||Constants 
        private const int SIZE = 2;
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

        private readonly Queue<Vector3> _generateMeshPosQueue = new();
        private readonly Queue<MeshData> _meshDataQueue = new();
        
        private readonly Dictionary<Vector3, (Mesh Mesh, Block[,,] Map)> _chunkMeshStore = new();
        private readonly Queue<Vector3> _chunkStoreHistory = new();
        
        //==================================================||Methods 

        private Task GetMeshData() {
            while (true) {
                if(_generateMeshPosQueue.Count == 0)
                    continue;

                var target = _generateMeshPosQueue.Dequeue();
                if(_chunkMeshStore.ContainsKey(target))
                    continue;

                _generator.Generate(_args, target);
                
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
        }
        
        private void Update() {
        }
    }
}