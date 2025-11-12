using UnityEngine;

namespace MapGenerator {
    /*public abstract class BiomeBase: IBiomeGenerator {

        protected abstract int[] _thinkBloca { get; }
        protected abstract Block[] _thinkBlock { get; }
        
        public abstract bool BiomeCheck();

        public virtual Block[] Generate(MapGenerationArgs pArgs, Vector3 pChunkIdx, Vector3 pPos) {

            var result = new Block[pArgs.ChunkHeight];
            var noise = new PerLinNoise(pArgs.Seed);
            pPos *= pArgs.Interval;
            pPos += pPos;
            
            for (int y = pArgs.ChunkHeight - 1; y >= 0; y--) {

                pPos.y = y * pArgs.Interval;
                var isAir = pArgs.CaveRange <= noise.Get(pPos, pArgs.Octave);
                if (isAir) {
                    result[y] = Block.Air;
                    continue;
                }

                var upIdx = pArgs.HeightLimit - y;
                _thinkBloca.GetLowerBound(upIdx);
                
                result[y] = isAir 
                    ? Block.Air 
                    : y == height - 1 
                        ? Block.Grass
                        : Block.Stone;
            }
        }
    }*/
}