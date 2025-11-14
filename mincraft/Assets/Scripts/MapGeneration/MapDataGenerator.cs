using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MapGenerator {
    public partial class MapMeshGenerator {

       //==================================================||Constances 
        private const int MIN_TREE_HEIGHT = 4;
        private const int MAX_TREE_HEIGHT = 4;
        private const float LEAF_VERTEX_ERASE_RATIO = 0.5f; 
        private static readonly int[] LEAF_RADIUSES = {2, 1};
        private static readonly int[] LEAF_HEIGHTS = {2, 2};
        
       //==================================================||Methods 
        public Block[,,] PerlinMapGeneration(MapGenerationArgs pArgs, Vector3Int pChunkIdx, out Queue<PaintInfo> paintingTarget) {
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
                var x = treePosRandom.Next(0, pArgs.ChunkLength);
                var y = treePosRandom.Next(0, pArgs.ChunkLength);
                var z = treePosRandom.Next(MIN_TREE_HEIGHT, MAX_TREE_HEIGHT + 1);
                GenerateTree(x, y, z, paintingTarget);
            }
                        
            return map;

            void GenerateTree(int pX, int pY, int height, Queue<PaintInfo> pPaintQueue) {
                var posY = 0;
                                    
                var isPlaceable = false;
                for (int j = pArgs.ChunkHeight - 1; j > -1; j--) {
                    var block = map[pX, j, pY];
                    if (block == Block.Air)
                        continue;
                                    
                    if (block == Block.Grass) {
                        isPlaceable = true;
                        posY = j + 1;
                    }
                    else
                        break;
                }

                if (!isPlaceable)
                    return;
                                    
                for (int j = 0; j < height && posY + j < pArgs.ChunkHeight; j++)
                    SetBlock(pX, posY + j, pY, Block.TreeBlock, pPaintQueue);

                var r = new Random((pX + pChunkIdx.x * pArgs.ChunkLength) * 100 + pY + pChunkIdx.y * pArgs.ChunkLength);
                var sum = -LEAF_HEIGHTS[0];
                for (int place = 0; place < LEAF_HEIGHTS.Length; place++) {
                    for (int leafY = 0; leafY < LEAF_HEIGHTS[place]; leafY++) {
                        for (int leafX = -LEAF_RADIUSES[place]; leafX <= LEAF_RADIUSES[place]; leafX++) {
                            for (int leafZ = -LEAF_RADIUSES[place]; leafZ <= LEAF_RADIUSES[place]; leafZ++) {

                                //not vertex
                                if ((leafX != -LEAF_RADIUSES[place] && leafX != LEAF_RADIUSES[place]) 
                                    || (leafZ != -LEAF_RADIUSES[place] && leafZ != LEAF_RADIUSES[place])) 
                                {
                                    SetBlock(pX + leafX, posY + height + leafY + sum, pY + leafZ, Block.Leaf, pPaintQueue);
                                    continue;
                                }
                                
                                //top vertex
                                if (place == LEAF_HEIGHTS.Length - 1 && leafY == LEAF_HEIGHTS[place] - 1) {
                                    continue;
                                }
                                
                                if(r.NextDouble() > LEAF_VERTEX_ERASE_RATIO)
                                    SetBlock(pX + leafX, posY + height + leafY + sum, pY + leafZ, Block.Leaf, pPaintQueue);
                            }
                        }
                    }
                
                    sum += LEAF_RADIUSES[place];
                }
            }
            
            void SetBlock(int pX, int pY, int pZ, Block pBlock, Queue<PaintInfo> pPaintQueue, bool pForce = false) {

                var chunkIdx = pChunkIdx;
                chunkIdx += new Vector3Int((int)((float)pX / pArgs.ChunkLength), 0, (int)((float)pZ / pArgs.ChunkLength));
                pX %= pArgs.ChunkLength;
                pZ %= pArgs.ChunkLength;
                if (pX < 0) {
                    pX += pArgs.ChunkLength;
                    chunkIdx.x--;
                }
                if (pZ < 0) {
                    pZ += pArgs.ChunkLength;
                    chunkIdx.z--;
                }

                if (chunkIdx == pChunkIdx) {
                    if(pForce || map[pX, pY, pZ] == Block.Air)
                        map[pX, pY, pZ] = pBlock;
                }
                else {
                    pPaintQueue.Enqueue(new(chunkIdx, new(pX, pY, pZ), pBlock, pForce));
                }
                
                if (pX == 0) {
                    var targetIdx = chunkIdx + Vector3Int.left;
                    pPaintQueue.Enqueue(new(targetIdx, new(pArgs.ChunkLength, pY, pZ), pBlock, pForce));
                }
                if (pZ == 0) {
                    var targetIdx = chunkIdx + Vector3Int.back;
                    pPaintQueue.Enqueue(new(targetIdx, new(pX, pY, pArgs.ChunkLength), pBlock, pForce));
                }
                                
            }
        }
    }
}