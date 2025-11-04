using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Extension;
using UnityEngine;

namespace MapGenerator {
    public partial class ChunkManager {
        
       //==================================================||Fields 
        //new chunks data
        private readonly ConcurrentStack<Vector3Int> _generateMapDataStack = new();
        
        //new chunks load - Multi Thread
        private readonly ConcurrentStack<Vector3Int> _generateMeshPosStack = new();
        private readonly ConcurrentStack<MeshData> _meshDataStack = new();
        
        //Rebake Load targets - Single Thread
        private readonly ConcurrentStack<Vector3Int> _rebakeMeshPosStack = new();
        private readonly ConcurrentStack<MeshData> _rebakeMeshDataStack = new();
        
        //Chunk store
        private readonly ConcurrentDictionary<Vector3Int, (Mesh Mesh, Block[,,] Map)> _chunkDataStore = new();
        private readonly Queue<Vector3Int> _chunkStoreHistory = new();
        
       //==================================================||Methods 
        private Task GetMapData() {
            while (true) {
                if (_isQuit)
                    return null;

                if (!_generateMapDataStack.TryPop(out var target) || _chunkDataStore.ContainsKey(target)) {
                    Thread.Sleep(1);
                    continue;
                }

                _chunkDataStore.TryAdd(target, (null, _generator.PerlinMapGeneration(_args, target)));
                _generateMeshPosStack.Push(target);
            }
        }

        private Task GetMeshData() {
            while (true) {
                if (_isQuit)
                    return null;

                if (!_generateMeshPosStack.TryPop(out var target)) {
                    Thread.Sleep(1);
                    continue;
                }

                _meshDataStack.Push(_generator.Generate(_chunkDataStore[target].Map, target));
            }
        }
        
        private Task GetRebakedMeshData() {
            while (true) {
                if (_isQuit)
                    return null;
                if (!_rebakeMeshPosStack.TryPop(out var target)) {
                    Thread.Sleep(1);
                    continue;
                }

                var data = _chunkDataStore[target].Map;
                _rebakeMeshDataStack.Push(_generator.Generate(data, target));
            }
        }
        
        private void RegisterMesh(MeshData pData) {
            var mesh = new Mesh();
            mesh.SetVertices(pData.Vertices);
            mesh.triangles = pData.Triangles;
            mesh.SetUVs(0, pData.Uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            _chunkStoreHistory.Enqueue(pData.Idx);
            _chunkDataStore[pData.Idx] = (mesh, pData.Map);
            while (_chunkStoreHistory.Count > MESH_RESTORE_LIMIT) {
                _chunkDataStore.Remove(_chunkStoreHistory.Dequeue(), out var _);
            }
        }

        private void LoadAllMesh() {
            var interval = new Vector3(_args.ChunkLength, 0, _args.ChunkLength);
                    
            foreach (var chunk in _chunks) {
                var idx = chunk.Idx;
                var pos = idx.Multiple(interval);
                pos -= new Vector3(_args.ChunkLength / 2f, 0, _args.ChunkLength / 2f);
        
                if(_chunkDataStore.TryGetValue(idx, out var value))
                    chunk.SetUp(value.Mesh, pos);
            }    
        }
    }
}