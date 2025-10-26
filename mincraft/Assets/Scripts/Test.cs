using UnityEngine;

namespace MapGenerator {
    
    public class Test: MonoBehaviour {

        [SerializeField] private Chunk _chunkPrefab;
        
        private MapEnv _generator;
        private MapGenerationArgs _args;
    private const int SIZE = 5;
        
        private void Awake() {
            _generator = new MapEnv();
            _args = new MapGenerationArgs(pOctave: 1);

            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
            for (int i = 0; i < SIZE; i++) {
                for (int j = 0; j < SIZE; j++) {
                    var pos = interval;
                    pos.x *= j;
                    pos.z *= i;
                    Generate(new (j, 0,  i), pos);
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