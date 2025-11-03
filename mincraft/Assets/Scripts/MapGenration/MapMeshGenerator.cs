using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using MapGenerator.Tile;
using UnityEngine;
using FaceType = MapGenerator.Tile.TileIdxData.FaceType;
using Random = System.Random;

namespace MapGenerator {

    
    public class MapMeshGenerator {

        public static readonly Vector3[] UP_FACE_PIVOTS;
        public static readonly Vector3[] DOWN_FACE_PIVOTS;
        public static readonly Vector3[] LEFT_FACE_PIVOTS;
        public static readonly Vector3[] RIGHT_FACE_PIVOTS;
        public static readonly Vector3[] FRONT_FACE_PIVOTS;
        public static readonly Vector3[] BEHIND_FACE_PIVOTS;
        public static readonly int[] TRIANGLE_PIVOTS = { 0, 1, 2, 0, 2, 3 };
        
        static MapMeshGenerator() {
            UP_FACE_PIVOTS = new Vector3[] { new(0, 1, 1), new(1, 1, 1), new(1, 1, 0), new(0, 1, 0) };
            DOWN_FACE_PIVOTS = new Vector3[] { new(1, 0, 0), new(1, 0, 1), new(0, 0, 1), new(0, 0, 0) };
            FRONT_FACE_PIVOTS = new Vector3[] { new(0, 1, 0), new(1, 1, 0), new(1, 0, 0), new(0, 0, 0) };
            BEHIND_FACE_PIVOTS = new Vector3[] { new(1, 0, 1), new(1, 1, 1), new(0, 1, 1), new(0, 0, 1) };
            LEFT_FACE_PIVOTS = new Vector3[] { new(0, 0, 1), new(0, 1, 1), new(0, 1, 0), new(0, 0, 0) };
            RIGHT_FACE_PIVOTS = new Vector3[] { new(1, 1, 0), new(1, 1, 1), new(1, 0, 1), new(1, 0, 0) };
        }

        public MeshData Generate(Block[,,] pMap, Vector3Int pPos) {
            var mesh = new MeshData();
            var (limitX, limitY, limitZ) = (pMap.GetLength(0), pMap.GetLength(1), pMap.GetLength(2));
            var visitCheck = new CheckDirection[limitX, limitY, limitZ];
 
            var triangles = new List<int>(1500);
            var vertices = new List<Vector3>(1500);
            var uvs = new List<Vector4>();
             
            for (int y = 0; y < limitY; y++) {
                for (int x = 0; x < limitX; x++) {
                    for (int z = 0; z < limitZ; z++) {
                        if (pMap[x, y, z] == Block.Air || visitCheck[x,y,z] == CheckDirection.All)
                            continue;
 
                        //generate direction mesh
                        GenerateX(LEFT_FACE_PIVOTS, CheckDirection.Left,
                            (x, y, z) => z == limitZ - 1 || x == 0 || pMap[x - 1, y, z] != Block.Air, 
                            x, y, z, FaceType.Side
                        );
                        GenerateX(RIGHT_FACE_PIVOTS, CheckDirection.Right,
                            (x, y, z) => z == limitZ - 1 || x == limitX - 1 || pMap[x + 1, y, z] != Block.Air, 
                            x, y, z, FaceType.Side
                        );
                         
                        GenerateY(UP_FACE_PIVOTS, CheckDirection.Up,
                            (x, y, z) =>  x == limitX - 1 || z == limitZ - 1 || y != limitY - 1 && pMap[x, y + 1, z] != Block.Air,
                            x, y, z, FaceType.Up
                        );
                        GenerateY(DOWN_FACE_PIVOTS, CheckDirection.Down,
                            (x, y, z) => x == limitX - 1 || z == limitZ - 1 || y != 0  && pMap[x, y - 1, z] != Block.Air, 
                            x, y, z, FaceType.Down
                        );
                        GenerateZ(FRONT_FACE_PIVOTS, CheckDirection.Front,
                            (x, y, z) => x == limitX - 1 || z == 0 || pMap[x, y, z - 1] != Block.Air, 
                            x, y, z, FaceType.Side
                        );
                        GenerateZ(BEHIND_FACE_PIVOTS, CheckDirection.Behind,
                            (x, y, z) => x == limitX - 1 || z == limitZ - 1 || pMap[x, y, z + 1] != Block.Air, 
                            x, y, z, FaceType.Side
                        );
                    }
                }
            }

            mesh.Idx = pPos;
            mesh.Vertices = vertices;
            mesh.Uvs = uvs;
            mesh.Triangles = triangles.ToArray();
            mesh.Map = pMap;
            return mesh;
 
            #region SubMethods
 
             
 
            void GenerateX(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z, FaceType pFace) {
 
                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");
 
                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
 
                var target = pMap[x, y, z];
                int lenghtY = limitY - y;
                for (int dy = 1; y + dy < limitY; dy++) {
                                     
                    var isEmpty = pMap[x, y + dy, z] != target || visitCheck[x, y + dy, z].HasFlag(pFlag);
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
                                         
                        var isEmpty = pMap[x, y + dy, z + dz] != target || visitCheck[x, y + dy, z + dz].HasFlag(pFlag);
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
                                 
                var textureIdx = TileIdxData.Get(target, pFace);
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
                uvs.AddRange(pPivotList.Select(pivot => new Vector4(pivot.z * lengthZ, pivot.y *  lenghtY, textureIdx.X, textureIdx.Y)));
            }
            void GenerateZ(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z, FaceType pFace) {
 
                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");
 
                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
 
                var target = pMap[x, y, z];
                int lenghtY = limitY - y;
                for (int dy = 1; y + dy < limitY; dy++) {
                                     
                    var isEmpty = pMap[x, y + dy, z] != target || visitCheck[x, y + dy, z].HasFlag(pFlag);
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
                                         
                        var isEmpty = pMap[x + dx, y + dy, z] != target || visitCheck[x + dx, y + dy, z].HasFlag(pFlag);
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
                                 
                var textureIdx = TileIdxData.Get(target, pFace);
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
                uvs.AddRange(pPivotList.Select(pivot => new Vector4(pivot.x * lengthX , pivot.y *  lenghtY, textureIdx.X, textureIdx.Y)));
            }
            void GenerateY(Vector3[] pPivotList, CheckDirection pFlag, Func<int, int, int, bool> pIsPreviousExist, int x, int y, int z, FaceType pFace) {
 
                if (!pFlag.IsFlag())
                    throw new Exception("pFlag never can be multiFlag");
                if (pIsPreviousExist is null)
                    throw new Exception("pIsPreviousExist never can be null");
 
                if (visitCheck[x, y, z].HasFlag(pFlag))
                    return;
                if (pIsPreviousExist.Invoke(x,y,z)) 
                    return;
 
                var target = pMap[x, y, z];
                int lenghtZ = limitZ - z;
                for (int dz = 1; z + dz < limitZ; dz++) {
                                     
                    var isEmpty = pMap[x, y, z + dz] != target || visitCheck[x, y, z + dz].HasFlag(pFlag);
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
                                         
                        var isEmpty = pMap[x + dx, y, z + dz] != target || visitCheck[x + dx, y, z + dz].HasFlag(pFlag);
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
 
                var textureIdx = TileIdxData.Get(target, pFace);
                triangles.AddRange(TRIANGLE_PIVOTS.Select(element => element + startPoint));
                uvs.AddRange(pPivotList.Select(pivot => new Vector4(pivot.x * lengthX, pivot.z * lenghtZ, textureIdx.X, textureIdx.Y)));
            }
             
            #endregion           
        }
        
        public Block[,,] PerlinMapGeneration(MapGenerationArgs pArgs, Vector3 pOrigin) {
            pOrigin *= pArgs.ChunkRange;
            
            var noise = new PerLinNoise(pArgs.Seed);
            var noise2 = new FastNoiseLite(pArgs.Seed);
            var map = new Block[pArgs.ChunkLength + 1, pArgs.ChunkHeight, pArgs.ChunkLength + 1];
            
            for (int z = 0; z < pArgs.ChunkLength + 1; z++) {
                for (int x = 0; x < pArgs.ChunkLength + 1; x++) {
                    var heightMapPos = new Vector2(x * pArgs.Interval, z * pArgs.Interval) + new Vector2(pOrigin.x, pOrigin.z);
                    var height = pArgs.BaseHeight + Mathf.FloorToInt(
                        (noise.Get(heightMapPos, pArgs.Octave) + 1) * pArgs.HeightLimit / 2
                    );

                    var pos = new Vector3(pArgs.Interval * x + pOrigin.x, (height - 1) * pArgs.Interval, pArgs.Interval * z + pOrigin.z);
                    var r = new Random((int)((pArgs.Seed * pos.x * 123433) % 194) ^ (int)(pos.z * 15332));
                    
                    var baseBlockCnt = r.Next(0, 5);
                    var dirtyBlockCnt = r.Next(3, 6);
                    var firstBlockFlag = false;
                    var firstBlockPos = -1;
                    for (int y = height - 1; y >= 0; y--, pos.y -= pArgs.Interval) {

                        if (y <= baseBlockCnt) {
                            map[x, y, z] = Block.Base;
                            continue;
                        }

                        var isAir = pArgs.CaveRange <= noise.Get(pos, pArgs.Octave);
                        if(isAir)
                            map[x, y, z] = Block.Air;
                        else if (!firstBlockFlag) {
                            map[x, y, z] = Block.Grass;
                            firstBlockFlag = true;
                            firstBlockPos = y;
                        }
                        else if(firstBlockPos - y <= dirtyBlockCnt) {
                            map[x, y, z] = Block.Dirty;
                        }
                        else {
                            map[x, y, z] = Block.Stone;
                        }
                    }
                }
            }
            /*var treePosRandom = new Random((int)(pOrigin.x * 1235) ^ (int)(pOrigin.z * 531));
            for (int i = 0; i < 10; i++) {
                var pos = treePosRandom()
            }*/

            return map;
        }
    }
}