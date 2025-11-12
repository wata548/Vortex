using UnityEngine;

namespace MapGenerator {
    public partial class ChunkManager  {
        private void OnDrawGizmos() {
            Gizmos.color= UnityEngine.Color.red;
            Gizmos.DrawWireCube((_playerChunk + Vector3.right * 0.5f) * Args.ChunkLength, new (0, 100, Args.ChunkLength));
            Gizmos.DrawWireCube((_playerChunk - Vector3.right * 0.5f) * Args.ChunkLength, new (0, 100, Args.ChunkLength));
            Gizmos.DrawWireCube((_playerChunk + Vector3.forward * 0.5f) * Args.ChunkLength, new (Args.ChunkLength, 100, 0));
            Gizmos.DrawWireCube((_playerChunk - Vector3.forward * 0.5f) * Args.ChunkLength, new (Args.ChunkLength, 100, 0));
        }
    }
}