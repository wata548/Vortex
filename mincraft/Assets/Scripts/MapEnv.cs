using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extension;
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

        public MapGenerationArgs(int pBaseHeight = 10, int pHeightLimit = 5, int pSeed = 181818, int pChunkRange = 10, float  pInterval = 0.1f, int pOctave = 1) {
            BaseHeight = pBaseHeight;
            HeightLimit = pHeightLimit;
            Seed = pSeed;
            ChunkRange = pChunkRange;
            Interval = pInterval;
            Octave = pOctave;
        }
    } 
    
    public class MapEnv {

        private static Vector3[] UP_FACE_PIVOTS;
        private static Vector3[] DOWN_FACE_PIVOTS;
        private static int[] TRIANGLE_PIVOTS = new[] { 0, 1, 2, 0, 2, 3 };
        
        public MapEnv() {
            UP_FACE_PIVOTS = new Vector3[]{ new(0, 1, 1), new(1, 1, 1), new(1, 1, 0), new(0, 1, 0) };
            DOWN_FACE_PIVOTS = new Vector3[]{  new(1, 0, 0), new(1, 0, 1), new(0, 0, 1), new(0, 0, 0)  };
        }
        
        public void Generate(Mesh pMesh, MapGenerationArgs pArgs, Vector3 pOrigin) {
            var perlinMap = PerlinMapGeneration(pArgs, pOrigin);
            var (limitX, limitY, limitZ) = (perlinMap.GetLength(0), perlinMap.GetLength(1), perlinMap.GetLength(2));

            var triangles = new List<int>();
            var vertices = new List<Vector3>();
            
            for (int y = 0; y < limitY; y++) {
                for (int x = 0; x < limitX; x++) {
                    for (int z = 0; z < limitZ; z++) {
                        if (perlinMap[x, y, z] is Block.Checked or Block.Air)
                            continue;
                        
                        GenerateHorizontal(UP_FACE_PIVOTS, (x, y, z) => y != limitY - 1 && perlinMap[x, y + 1, z] != Block.Air, x, y, z);
                        GenerateHorizontal(DOWN_FACE_PIVOTS, (x, y, z) => y != 0  && perlinMap[x, y - 1, z] != Block.Air, x, y, z);
                        
                    }
                }
            }

            pMesh.vertices = vertices.ToArray();
            pMesh.triangles = triangles.ToArray();
            pMesh.RecalculateNormals();
            pMesh.RecalculateBounds();

            void GenerateHorizontal(Vector3[] pPivotList, [NotNull] Func<int, int, int, bool> isCeilExist, int x, int y, int z) {
                if (isCeilExist.Invoke(x,y,z)) 
                    return;
                                
                int lenghtZ = limitZ - z;
                for (int dz = 1; z + dz < limitZ; dz++) {
                                    
                    var isEmpty = perlinMap[x, y, z + dz] is Block.Air or Block.Checked;
                    if (isCeilExist.Invoke(x, y,z + dz) || isEmpty) {
                        lenghtZ = dz;
                        break;
                    }
                                
                    perlinMap[x, y, z + dz] = Block.Checked;
                }
                                
                int widthX = limitX - x;
                bool endFlag = false;
                for (int dx = 1; dx + x < limitX; dx++) {
                    for (int dz = 0; dz < lenghtZ; dz++) {
                                        
                        var isEmpty = perlinMap[x + dx, y, z + dz] is Block.Air or Block.Checked;
                        if (isCeilExist.Invoke(x + dx, y, z + dz) || isEmpty) {
                            widthX = dx;
                            endFlag = true;
                            break;
                        }
                    }
                                
                    if (endFlag) {
                        break;
                    }
                    for (int dz = 0; dz < lenghtZ; dz++) {
                        perlinMap[x + dx, y, z + dz] = Block.Checked;
                    }
                }
                                                            
                var size = new Vector3(widthX, 1, lenghtZ);
                var pos = new Vector3(x, y, z);
                var startPoint = vertices.Count;
                                                            
                foreach (var pivot in pPivotList) {
                    vertices.Add(pos + pivot.Multiple(size));
                }
                                
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
            }
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