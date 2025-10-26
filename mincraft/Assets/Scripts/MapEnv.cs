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
    
    public class MapEnv {

        [Flags]
        private enum CheckDirection: byte {
            None    = 0b000000,
            Up      = 0b100000,
            Down    = 0b010000,
            Left    = 0b001000,
            Right   = 0b000100,
            Front   = 0b000010,
            Behind  = 0b000001,
            All     = 0b111111,
        }
        
        private static Vector3[] UP_FACE_PIVOTS;
        private static Vector3[] DOWN_FACE_PIVOTS;
        private static Vector3[] LEFT_FACE_PIVOTS;
        private static Vector3[] RIGHT_FACE_PIVOTS;
        private static Vector3[] FRONT_FACE_PIVOTS;
        private static Vector3[] BEHIND_FACE_PIVOTS;
        private static int[] TRIANGLE_PIVOTS = { 0, 1, 2, 0, 2, 3 };
        
        public MapEnv() {
            UP_FACE_PIVOTS = new Vector3[] { new(0, 1, 1), new(1, 1, 1), new(1, 1, 0), new(0, 1, 0) };
            DOWN_FACE_PIVOTS = new Vector3[] { new(1, 0, 0), new(1, 0, 1), new(0, 0, 1), new(0, 0, 0) };
            FRONT_FACE_PIVOTS = new Vector3[] { new(0, 1, 0), new(1, 1, 0), new(1, 0, 0), new(0, 0, 0) };
            BEHIND_FACE_PIVOTS = new Vector3[] { new(1, 0, 1), new(1, 1, 1), new(0, 1, 1), new(0, 0, 1) };
            LEFT_FACE_PIVOTS = new Vector3[] { new(0, 0, 1), new(0, 1, 1), new(0, 1, 0), new(0, 0, 0) };
            RIGHT_FACE_PIVOTS = new Vector3[] { new(1, 1, 0), new(1, 1, 1), new(1, 0, 1), new(1, 0, 0) };
        }
        
        public Mesh Generate(MapGenerationArgs pArgs, Vector3 pOrigin) {
            var mesh = new Mesh();
            pOrigin -= new Vector3(pArgs.Interval, 0, pArgs.Interval);
            var perlinMap = PerlinMapGeneration(pArgs, pOrigin, pArgs.CaveRange);
            var (limitX, limitY, limitZ) = (perlinMap.GetLength(0), perlinMap.GetLength(1), perlinMap.GetLength(2));
            var visitCheck = new CheckDirection[limitX, limitY, limitZ];

            var triangles = new List<int>();
            var vertices = new List<Vector3>();
            
            for (int y = 0; y < limitY; y++) {
                for (int x = 0; x < limitX; x++) {
                    for (int z = 0; z < limitZ; z++) {
                        if (perlinMap[x, y, z] == Block.Air || visitCheck[x,y,z] == CheckDirection.All)
                            continue;

                        GenerateX(LEFT_FACE_PIVOTS, CheckDirection.Left,
                            (x, y, z) => z == 0 || z == limitZ - 1 || x == 0 || perlinMap[x - 1, y, z] != Block.Air, 
                            x, y, z
                        );
                        GenerateX(RIGHT_FACE_PIVOTS, CheckDirection.Right,
                            (x, y, z) => z == 0 || z == limitZ - 1 || x == limitX - 1 || perlinMap[x + 1, y, z] != Block.Air, 
                            x, y, z
                        );
                        
                        GenerateY(UP_FACE_PIVOTS, CheckDirection.Up,
                            (x, y, z) =>  x == 0 || z == 0 || x == limitX - 1 || z == limitZ - 1 || y != limitY - 1 && perlinMap[x, y + 1, z] != Block.Air,
                            x, y, z
                        );
                        GenerateY(DOWN_FACE_PIVOTS, CheckDirection.Down,
                            (x, y, z) => x == 0 || z == 0 || x == limitX - 1 || z == limitZ - 1 || y != 0  && perlinMap[x, y - 1, z] != Block.Air, 
                            x, y, z
                        );
                        GenerateZ(FRONT_FACE_PIVOTS, CheckDirection.Front,
                            (x, y, z) => x == 0 || x == limitX - 1 || z == 0 || perlinMap[x, y, z - 1] != Block.Air, 
                            x, y, z
                        );
                        GenerateZ(BEHIND_FACE_PIVOTS, CheckDirection.Behind,
                            (x, y, z) => x == 0 || x == limitX - 1 || z == limitZ - 1 || perlinMap[x, y, z + 1] != Block.Air, 
                            x, y, z
                        );
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
            void GenerateX(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z) {

                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");

                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
                                
                int lenghtY = limitY - y;
                for (int dy = 1; y + dy < limitY; dy++) {
                                    
                    var isEmpty = perlinMap[x, y + dy, z] == Block.Air || visitCheck[x, y + dy, z].HasFlag(pFlag);
                    if (pIsPreviousExist.Invoke(x, y + dy, z) || isEmpty) {
                        lenghtY = dy;
                        break;
                    }
                                
                    visitCheck[x, y + dy, z] |= pFlag;
                }
                                
                int lengthZ = limitZ - z;
                bool endFlag = false;
                for (int dz = 1; dz + z < limitZ; dz++) {
                    for (int dy = 0; dy < lenghtY; dy++) {
                                        
                        var isEmpty = perlinMap[x, y + dy, z + dz] == Block.Air || visitCheck[x, y + dy, z + dz].HasFlag(pFlag);
                        if (pIsPreviousExist.Invoke(x, y + dy, z + dz) || isEmpty) {
                            lengthZ = dz;
                            endFlag = true;
                            break;
                        }
                    }
                                
                    if (endFlag) {
                        break;
                    }
                    for (int dy = 0; dy < lenghtY; dy++) {
                        visitCheck[x, y + dy, z + dz] |= pFlag;
                    }
                }
                                                            
                var size = new Vector3(1, lenghtY, lengthZ);
                var pos = new Vector3(x, y, z);
                var startPoint = vertices.Count;
                                                            
                foreach (var pivot in pPivotList) {
                    vertices.Add(pos + pivot.Multiple(size));
                }
                                
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
            }
            void GenerateZ(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z) {

                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");

                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
                                
                int lenghtY = limitY - y;
                for (int dy = 1; y + dy < limitY; dy++) {
                                    
                    var isEmpty = perlinMap[x, y + dy, z] == Block.Air || visitCheck[x, y + dy, z].HasFlag(pFlag);
                    if (pIsPreviousExist.Invoke(x, y + dy, z) || isEmpty) {
                        lenghtY = dy;
                        break;
                    }
                                
                    visitCheck[x, y + dy, z] |= pFlag;
                }
                                
                int lengthX = limitX - x;
                bool endFlag = false;
                for (int dx = 1; dx + x < limitX; dx++) {
                    for (int dy = 0; dy < lenghtY; dy++) {
                                        
                        var isEmpty = perlinMap[x + dx, y + dy, z] == Block.Air || visitCheck[x + dx, y + dy, z].HasFlag(pFlag);
                        if (pIsPreviousExist.Invoke(x + dx, y + dy, z) || isEmpty) {
                            lengthX = dx;
                            endFlag = true;
                            break;
                        }
                    }
                                
                    if (endFlag) {
                        break;
                    }
                    for (int dy = 0; dy < lenghtY; dy++) {
                        visitCheck[x + dx, y + dy, z] |= pFlag;
                    }
                }
                                                            
                var size = new Vector3(lengthX, lenghtY, 1);
                var pos = new Vector3(x, y, z);
                var startPoint = vertices.Count;
                                                            
                foreach (var pivot in pPivotList) {
                    vertices.Add(pos + pivot.Multiple(size));
                }
                                
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
            }
            void GenerateY(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z) {

                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");

                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
                                
                int lenghtZ = limitZ - z;
                for (int dz = 1; z + dz < limitZ; dz++) {
                                    
                    var isEmpty = perlinMap[x, y, z + dz] == Block.Air || visitCheck[x, y, z + dz].HasFlag(pFlag);
                    if (pIsPreviousExist.Invoke(x, y,z + dz) || isEmpty) {
                        lenghtZ = dz;
                        break;
                    }
                                
                    visitCheck[x, y, z + dz] |= pFlag;
                }
                                
                int lengthX = limitX - x;
                bool endFlag = false;
                for (int dx = 1; dx + x < limitX; dx++) {
                    for (int dz = 0; dz < lenghtZ; dz++) {
                                        
                        var isEmpty = perlinMap[x + dx, y, z + dz] == Block.Air || visitCheck[x + dx, y, z + dz].HasFlag(pFlag);
                        if (pIsPreviousExist.Invoke(x + dx, y, z + dz) || isEmpty) {
                            lengthX = dx;
                            endFlag = true;
                            break;
                        }
                    }
                                
                    if (endFlag) {
                        break;
                    }
                    for (int dz = 0; dz < lenghtZ; dz++) {
                        visitCheck[x + dx, y, z + dz] |= pFlag;
                    }
                }
                                                            
                var size = new Vector3(lengthX, 1, lenghtZ);
                var pos = new Vector3(x, y, z);
                var startPoint = vertices.Count;
                                                            
                foreach (var pivot in pPivotList) {
                    vertices.Add(pos + pivot.Multiple(size));
                }
                                
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
            }
        }

        private Block[,,] PerlinMapGeneration(MapGenerationArgs pArgs, Vector3 pOrigin, float pCaveRange) {
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
                        var isAir = pCaveRange <= noise.Get(pos, pArgs.Octave);
                        map[x, y, z] = isAir ? Block.Air : Block.DefaultBlock;
                    }
                }
            }

            return map;
        }
    }
}