using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public static class MapDataManager {
        private static ConcurrentDictionary<Vector3Int, Block[,,]> _mapDatas = new();
        private static Dictionary<Vector3Int, Mesh> _mapMesh = new();
        public static ConcurrentStack<Vector3Int> _refreshMeshCandidate = new();

        public static void SetMesh(Vector3Int pIdx, Mesh pMesh) {
            if (!_mapMesh.TryAdd(pIdx, pMesh))
                _mapMesh[pIdx] = pMesh;
        }  
        
        public static Block[,,] GetMapData(Vector3Int pIdx) {
            return _mapDatas[pIdx];
        }
            
        public static void AddMapData(Vector3Int pIdx, Block[,,] pMaps) {
            if(!_mapDatas.TryAdd(pIdx, pMaps))
                _mapDatas[pIdx] = pMaps;
        }
    }
}