using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public struct MapGenerationArgs {
        public int BaseHeight;
        public int HeightLimit;
        public int Seed;
        public int ChunkRange;
        public int Octave;
        public float Interval;

        public int ChunkLength => (int)(ChunkRange / Interval);
        public int ChunkHeight => BaseHeight + HeightLimit;

        public MapGenerationArgs(int pBaseHeight = 15, int pHeightLimit = 8, int pSeed = 181818, int pChunkRange = 10, float  pInterval = 0.1f, int pOctave = 1) {
            BaseHeight = pBaseHeight;
            HeightLimit = pHeightLimit;
            Seed = pSeed;
            ChunkRange = pChunkRange;
            Interval = pInterval;
            Octave = pOctave;
        }
    } 
    
    public class MapEnv {

        private static Vector3[] UPPER_PIVOTS;
        
        public MapEnv() {
            UPPER_PIVOTS = new Vector3[]{ new(0, 1, 1), new(1, 1, 1), new(1, 1, 0), new(0, 1, 0) };
        }
        
        public void Generate(Mesh pMesh, MapGenerationArgs pArgs, Vector3 pOrigin) {
            var perlinMap = PerlinMapGeneration(pArgs, pOrigin);
            var (limitX, limitY, limitZ) = (perlinMap.GetLength(0), perlinMap.GetLength(1), perlinMap.GetLength(2));

            var triangles = new List<int>();
            var vertices = new List<Vector3>();
            
            for (int y = 0; y < limitY; y++) {
                for (int x = 0; x < limitX; x++) {
                    for (int z = 0; z < limitZ; z++) {
                        if (perlinMap[x, limitY - 1 - y, z] is Block.Checked or Block.Air)
                            continue;

                        var pos = new Vector3(x, y, z);
                        var startPoint = vertices.Count;
                        if (y == 0 || perlinMap[x, limitY - y, z] == Block.Air) {
                            foreach (var pivot in UPPER_PIVOTS) {
                                vertices.Add(pos + pivot);
                            }

                            triangles.AddRange(new[] {startPoint, startPoint + 1, startPoint + 2 });
                            triangles.AddRange(new[] { startPoint, startPoint + 2, startPoint + 3 });
                        }
                    }
                }
            }

            pMesh.vertices = vertices.ToArray();
            pMesh.triangles = triangles.ToArray();
            pMesh.RecalculateNormals();
            pMesh.RecalculateBounds();
        }

        private Block[,,] PerlinMapGeneration(MapGenerationArgs pArgs, Vector3 pOrigin) {
            var noise = new PerLinNoise(pArgs.Seed);
            var map = new Block[pArgs.ChunkLength, pArgs.ChunkHeight, pArgs.ChunkLength];

            for (int z = 0; z < pArgs.ChunkLength; z++) {
                for (int x = 0; x < pArgs.ChunkLength; x++) {
                    var heightMapPos = new Vector2(x * pArgs.Interval, z * pArgs.Interval) + new Vector2(pOrigin.x, pOrigin.z);
                    var height = pArgs.BaseHeight + Mathf.FloorToInt(
                        (noise.Get(heightMapPos, pArgs.Octave) + 1) * pArgs.HeightLimit / 2
                    );
                    for (int y = 0; y < height; y++) {

                        var pos = new Vector3(x * pArgs.Interval, y * pArgs.Interval, z * pArgs.Interval) + pOrigin;
                        var isAir = 0 < noise.Get(pos, pArgs.Octave);
                        map[x, y, z] = isAir ? Block.Air : Block.DefaultBlock;
                    }
                }
            }

            return map;
        }
    }
}