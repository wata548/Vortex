using UnityEngine;

namespace MapGenerator {
    public partial class ChunkManager  {
        private void OnDrawGizmos() {
            Gizmos.color= UnityEngine.Color.red;
            Gizmos.DrawWireCube((_playerChunk + Vector3.right * 0.5f) * _args.ChunkLength, new (0, 100, _args.ChunkLength));
            Gizmos.DrawWireCube((_playerChunk - Vector3.right * 0.5f) * _args.ChunkLength, new (0, 100, _args.ChunkLength));
            Gizmos.DrawWireCube((_playerChunk + Vector3.forward * 0.5f) * _args.ChunkLength, new (_args.ChunkLength, 100, 0));
            Gizmos.DrawWireCube((_playerChunk - Vector3.forward * 0.5f) * _args.ChunkLength, new (_args.ChunkLength, 100, 0));
        }
    }
}