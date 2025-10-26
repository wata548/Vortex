using UnityEngine;

namespace MapGenerator {
    
    public class Test: MonoBehaviour {

        [SerializeField] private Chunk _chunkPrefab;
        
        private MapMeshGenerator _generator;
        private MapGenerationArgs _args;
        private const int SIZE = 5;

        private Chunk[,] _chunks = new Chunk[SIZE,SIZE];
        
        private void Awake() {
            _generator = new MapMeshGenerator();
            _args = new MapGenerationArgs(pOctave: 2);

            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
            for (int i = 0; i < SIZE; i++) {
                for (int j = 0; j < SIZE; j++) {
                    var pos = interval;
                    pos.x *= j;
                    pos.z *= i;
                    _chunks[i,j] = Generate(new (j, 0,  i), pos);
                }
            }
        }

        private Chunk Generate(Vector3 pChunkPos, Vector3 pPos) {
            var pos = pChunkPos * _args.ChunkRange;
            var mesh = _generator.Generate(_args, pos);
            var chunk = Instantiate(_chunkPrefab);
            chunk.SetUp(mesh, pPos);

            return chunk;
        } 
    }
}