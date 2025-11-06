using Extension;
using UnityEngine;

namespace MapGenerator {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk: MonoBehaviour {

        public Vector3 Pos { get; private set; }
        public Vector3Int Idx { get; private set; }
        public Mesh Mesh => GetComponent<MeshFilter>().mesh;
        
        public void Ready(Vector3Int pIdx) => Idx = pIdx;
        
        public void SetUp(Mesh pMesh, Vector3 pPos) {

            Pos = pPos;
            transform.position = pPos;
            
            var meshFilter = GetComponent<MeshFilter>();
            var meshCollider = GetComponent<MeshCollider>();
            
            meshFilter.mesh = pMesh;
            meshCollider.sharedMesh = pMesh;    
        }
        
        public static Vector3Int GetChunkIdx(MapGenerationArgs pArgs, Vector3 pPos) {
            var x = pPos.x / pArgs.ChunkLength;
            x = x.Sign() * (Mathf.Abs(x) + 0.5f);
            var z = pPos.z / pArgs.ChunkLength;
            z = z.Sign() * (Mathf.Abs(z) + 0.5f);

            return new((int)x, 0, (int)z);
        }
        
        public static Vector3 GetChunkLocalPos(MapGenerationArgs pArgs, Vector3 pPos) {
            var chunk = GetChunkIdx(pArgs, pPos);
            var temp = Vector3.zero;
            
            temp.x = chunk.x - 0.5f;
            temp.z = chunk.z - 0.5f;
            temp *= pArgs.ChunkLength;
            pPos -= temp;
            
            return pPos;
        }
        
    }
}