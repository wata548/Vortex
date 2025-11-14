using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entity.Enemy;
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
        
        //Re bake Load targets - Single Thread
        private readonly ConcurrentStack<Vector3Int> _reBakeMeshPosStack = new();
        private readonly ConcurrentStack<MeshData> _reBakeMeshDataStack = new();
        
        //Chunk store
        private readonly ConcurrentDictionary<Vector3Int, (Mesh Mesh, Block[,,] Map)> _chunkDataStore = new();
        
        private readonly ConcurrentQueue<Queue<(Vector3Int Chunk, Vector3Int Pos, Block Block)>> _paintingTempData = new();
        private readonly Dictionary<Vector3Int, Queue<(Vector3Int Pos, Block Block)>> _paintingData = new();
        
       //==================================================||Methods 
       private void PaintOtherChunk() {

           while (_paintingTempData.Count > 0) {
               _paintingTempData.TryDequeue(out var queue);
               while (queue.Count > 0) {
                   var data = queue.Dequeue();
                   _paintingData.TryAdd(data.Chunk, new());
                   _paintingData[data.Chunk].Enqueue((data.Pos, data.Block));
               }
           }
           
           var targetChunks = new HashSet<Vector3Int>();
           foreach (var targetChunk in _paintingData) {
               if(!_chunkDataStore.ContainsKey(targetChunk.Key) || _chunkDataStore[targetChunk.Key].Mesh == null)
                   continue;

               foreach (var data in targetChunk.Value) {
                   _chunkDataStore[targetChunk.Key].Map[data.Pos.x, data.Pos.y, data.Pos.z] = data.Block;
               }
               targetChunks.Add(targetChunk.Key);
           }

           foreach (var chunk in targetChunks) {
               _paintingData.Remove(chunk);
               _generateMeshPosStack.Push(chunk);
           }
       }
       
        private Task GetMapData() {
            while (true) {
                if (_isQuit)
                    return null;

                if (!_generateMapDataStack.TryPop(out var target) || _chunkDataStore.ContainsKey(target)) {
                    Thread.Sleep(1);
                    continue;
                }

                _chunkDataStore.TryAdd(target, (null, _generator.PerlinMapGeneration(Args, target, out var updateTarget)));
                _paintingTempData.Enqueue(updateTarget);
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
        
        private Task GetReBakedMeshData() {
            while (true) {
                if (_isQuit)
                    return null;
                if (!_reBakeMeshPosStack.TryPop(out var target)) {
                    Thread.Sleep(1);
                    continue;
                }

                var data = _chunkDataStore[target].Map;
                _reBakeMeshDataStack.Push(_generator.Generate(data, target));
            }
        }
        
        private void RegisterMesh(MeshData pData) {
            var mesh = new Mesh();
            mesh.SetVertices(pData.Vertices);
            mesh.triangles = pData.Triangles;
            mesh.SetUVs(0, pData.Uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            _chunkDataStore[pData.Idx] = (mesh, pData.Map);
        }

        private void LoadAllMesh() {
            var interval = new Vector3(Args.ChunkLength, 0, Args.ChunkLength);
                    
            foreach (var chunk in _chunks) {
                var idx = chunk.Idx;
                var pos = idx.Multiple(interval);
                pos -= new Vector3(Args.ChunkLength / 2f, 0, Args.ChunkLength / 2f);
        
                if(_chunkDataStore.TryGetValue(idx, out var value))
                    chunk.SetUp(value.Mesh, pos);
            }    
        }
    }
}