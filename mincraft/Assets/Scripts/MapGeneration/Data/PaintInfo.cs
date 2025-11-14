using UnityEngine;

namespace MapGenerator {
    public struct PaintInfo {
        public Vector3Int Chunk;
        public Vector3Int Pos;
        public Block Block;
        public bool Force;

        public PaintInfo(Vector3Int pChunk, Vector3Int pPos, Block pBlock, bool pForce) {
            Chunk = pChunk;
            Pos = pPos;
            Block = pBlock;
            Force = pForce;
        }
    }
}