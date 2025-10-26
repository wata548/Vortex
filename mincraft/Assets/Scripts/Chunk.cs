using UnityEngine;

namespace MapGenerator {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk: MonoBehaviour {

        public Vector3 Pos { get; private set; }
        
        public void SetUp(Mesh pMesh, Vector3 pPos) {

            Pos = pPos;
            transform.position = pPos;
            
            var meshFilter = GetComponent<MeshFilter>();
            var meshCollider = GetComponent<MeshCollider>();
            
            meshFilter.mesh = pMesh;
            meshCollider.sharedMesh = pMesh;    
        }
        
    }
}