using UnityEngine;

namespace MapGenerator {
    public interface IBiomeGenerator {

        public bool BiomeCheck();
        public Block[] Generate(MapGenerationArgs pArgs, Vector3 pChunkIdx, Vector3 pPos);
    }
}