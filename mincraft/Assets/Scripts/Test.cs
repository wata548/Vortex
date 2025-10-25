using UnityEngine;

namespace MapGenerator {
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Test: MonoBehaviour {

        private void Awake() {
            var generator = new MapEnv();
            var args = new MapGenerationArgs(pOctave: 2);

            var mesh = new Mesh();
            var meshFilter = GetComponent<MeshFilter>();
            
            generator.Generate(mesh, args, Vector3.zero);
            meshFilter.mesh = mesh;
        } 
    }
}