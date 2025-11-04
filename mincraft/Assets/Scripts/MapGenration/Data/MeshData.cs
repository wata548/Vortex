using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public struct MeshData {

        public Vector3Int Idx;
        public Block[,,] Map;
        public List<Vector4> Uvs;
        public int[] Triangles;
        public List<Vector3> Vertices;
    }
}