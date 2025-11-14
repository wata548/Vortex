using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MapGenerator {
    public partial class MapMeshGenerator {

        
        public Block[,,] PerlinMapGeneration(MapGenerationArgs pArgs, Vector3Int pChunkIdx, out Queue<(Vector3Int Chunk, Vector3Int Pos, Block Block)> paintingTarget) {
            var map = new Block[pArgs.ChunkLength + 1, pArgs.ChunkHeight, pArgs.ChunkLength + 1];
            
            pChunkIdx *= pArgs.ChunkRange;
                                
            var noise = new PerLinNoise(pArgs.Seed);
            paintingTarget = new();
                                
            for (int z = 0; z < pArgs.ChunkLength + 1; z++) {
                for (int x = 0; x < pArgs.ChunkLength + 1; x++) {
                    var heightMapPos = new Vector2(x * pArgs.Interval + pChunkIdx.x, z * pArgs.Interval + pChunkIdx.z);
                    var height = pArgs.BaseHeight + Mathf.FloorToInt(
                        (noise.Get(heightMapPos, pArgs.Octave) + 1) * pArgs.HeightLimit / 2
                    );
                    
                    var pos = new Vector3(pArgs.Interval * x + pChunkIdx.x, (height - 1) * pArgs.Interval, pArgs.Interval * z + pChunkIdx.z);
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
                            map[x, y, z] = y > pArgs.BaseHeight ? Block.Grass : Block.Stone;
                            firstBlockFlag = true;
                            firstBlockPos = y;
                        }
                        else if(firstBlockPos - y <= dirtyBlockCnt) {
                            map[x, y, z] = y > pArgs.BaseHeight ? Block.Dirty : Block.Stone;
                        }
                        else {
                            map[x, y, z] = Block.Stone;
                        }
                    }
                }
            }
            var treePosRandom = new Random((int)(pArgs.Seed * pChunkIdx.x) ^ (int)(pChunkIdx.z + pArgs.Seed));
            for (int i = 0; i < pArgs.TreeTryCount; i++) {
                var posX = treePosRandom.Next(0, pArgs.ChunkLength);
                var posZ = treePosRandom.Next(0, pArgs.ChunkLength);
                var posY = 0;
                    
                var isPlaceable = false;
                for (int j = pArgs.ChunkHeight - 1; j > -1; j--) {
                    var block = map[posX, j, posZ];
                    if (block == Block.Air)
                        continue;
                    
                    if (block == Block.Grass) {
                        isPlaceable = true;
                        posY = j + 1;
                    }
                    else
                        break;
                }
                if(!isPlaceable)
                    continue;
                    
                for (int j = 0; j < 4 && posY + j < pArgs.ChunkHeight; j++)
                    SetBlock(posX, posY + j, posZ, Block.TreeBlock, paintingTarget);
                SetBlock(posX, posY + 4, posZ, Block.Leaf, paintingTarget);
            }
                        
            return map;
            
            void SetBlock(int pX, int pY, int pZ, Block pBlock, Queue<(Vector3Int Chunk, Vector3Int Pos, Block Block)> targetQueue) {
            
                map[pX, pY, pZ] = pBlock;
                if (pX == 0) {
                    var targetIdx = pChunkIdx + Vector3Int.left;
                    var pos = new Vector3Int(pArgs.ChunkLength, pY, pZ);
                    targetQueue.Enqueue((targetIdx, pos, pBlock));
                }
                if (pZ == 0) {
                    var targetIdx = pChunkIdx + Vector3Int.back;
                    var pos = new Vector3Int(pX, pY, pArgs.ChunkLength);
                    targetQueue.Enqueue((targetIdx, pos, pBlock));
                }
                                
            }
        }
    }
}