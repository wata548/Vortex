using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using MapGenerator;
using MapGenerator.Tile;
using UnityEngine;

namespace Test {
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class TestMeshGenerator: MonoBehaviour {
        [SerializeField] private Block _targetBlock = Block.Air;
        private Block _temp = Block.Air;
        
        private Mesh MeshGenerate() {
            if (_targetBlock == Block.Air)
                return null;
            var _ = new MapMeshGenerator();
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector4>();

            Func<Vector3, Vector4> toUvX = (pivot) => {
                var (x, y) = TileIdxData.Get(_targetBlock, FaceType.Side);
                return new Vector4(pivot.z, pivot.y, x, y);
            };
            Func<Vector3, Vector4> toUvZ = (pivot) => {
                var (x, y) = TileIdxData.Get(_targetBlock, FaceType.Side);
                return new Vector4(pivot.x, pivot.y, x, y);
            };
            MakeFace(MapMeshGenerator.UP_FACE_PIVOTS,
                (pivot) => {
                    var (x, y) = TileIdxData.Get(_targetBlock, FaceType.Up);
                    return new Vector4(pivot.x, pivot.z, x, y);
                });
            MakeFace(MapMeshGenerator.DOWN_FACE_PIVOTS,
                (pivot) => {
                    var (x, y) = TileIdxData.Get(_targetBlock, FaceType.Down);
                    return new Vector4(pivot.x, pivot.z, x, y);
                });
            MakeFace(MapMeshGenerator.LEFT_FACE_PIVOTS, toUvX);
            MakeFace(MapMeshGenerator.RIGHT_FACE_PIVOTS, toUvX);
            MakeFace(MapMeshGenerator.FRONT_FACE_PIVOTS, toUvZ);
            MakeFace(MapMeshGenerator.BEHIND_FACE_PIVOTS, toUvZ);

            mesh.SetVertices(vertices);
            mesh.triangles = triangles.ToArray();
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
            
            void MakeFace(Vector3[] pPivots, Func<Vector3, Vector4> pToUv) {
                var verticesCnt = vertices.Count;
                
                vertices.AddRange(pPivots);
                uvs.AddRange(pPivots.Select(pToUv));
                triangles.AddRange(MapMeshGenerator.TRIANGLE_PIVOTS
                    .Select(pivot => pivot + verticesCnt)
                );
            } 
        }
        
        private void Update() {
            if (_targetBlock == _temp)
                return;

            _temp = _targetBlock;
            GetComponent<MeshFilter>().mesh = MeshGenerate();
        }
    }
}