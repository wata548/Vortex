using UnityEngine;

namespace MapGenerator {
    /*public class HillsGenrator: IBiomeGenerator {
        
        public Block[,,] Generate(MapGenerationArgs pArgs, Vector3 pOrigin) {
            var noise = new PerLinNoise(pArgs.Seed);
            var map = new Block[pArgs.ChunkLength + 2, pArgs.ChunkHeight, pArgs.ChunkLength + 2];

            for (int z = 0; z < pArgs.ChunkLength + 2; z++) {
                for (int x = 0; x < pArgs.ChunkLength + 2; x++) {
                    var heightMapPos = new Vector2(x * pArgs.Interval, z * pArgs.Interval) + new Vector2(pOrigin.x, pOrigin.z);
                    var height = pArgs.BaseHeight + Mathf.FloorToInt(
                        (noise.Get(heightMapPos, pArgs.Octave) + 1) * pArgs.HeightLimit / 2
                    );

                    for (int y = 0; y < height; y++) {

                        var pos = new Vector3(x * pArgs.Interval, y * pArgs.Interval, z * pArgs.Interval) + pOrigin;
                        var isAir = pArgs.CaveRange <= noise.Get(pos, pArgs.Octave);
                        map[x, y, z] = isAir 
                            ? Block.Air 
                            : y == height - 1 
                                ? Block.Grass
                                : Block.Stone;
                    }
                }
            }

            return map;
        }
    }*/
}