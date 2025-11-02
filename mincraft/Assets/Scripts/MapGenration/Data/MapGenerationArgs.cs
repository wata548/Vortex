using System;
using UnityEngine;

namespace MapGenerator {
    [Serializable]
    public struct MapGenerationArgs {
        public int BaseHeight;
        public int HeightLimit;
        public int Seed;
        public int ChunkRange;
        public int Octave;
        public float Interval;
        public float CaveRange;
        
        public int ChunkLength => (int)(ChunkRange / Interval);
        public int ChunkHeight => BaseHeight + HeightLimit;

        public MapGenerationArgs(
            int pBaseHeight = 16,
            int pHeightLimit = 16,
            int pSeed = 181818,
            int pChunkRange = 1,
            float  pInterval = 0.0625f,
            int pOctave = 1,
            float pCaveRange = 0.3f
        ) {
            BaseHeight = pBaseHeight;
            HeightLimit = pHeightLimit;
            Seed = pSeed;
            ChunkRange = pChunkRange;
            Interval = pInterval;
            Octave = pOctave;
            CaveRange = pCaveRange;
        }
    }
}