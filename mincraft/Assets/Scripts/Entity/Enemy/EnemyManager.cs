using System.Collections.Generic;
using System.Linq;
using Extension;
using Extension.Test;
using MapGenerator;
using UnityEngine;

namespace Entity.Enemy {
    public static class EnemyManager {

        private static Dictionary<Vector3Int, List<EnemyData>> _enemies = new();
        private static List<EnemyBase> _loadedEnemies = new();

        [TestMethod(runtimeOnly: true)]
        public static void Spawn(int x = 4, int y = 60, int z = 4, string enemy = "Enemy") {
            var data = new EnemyData(enemy, x, y, z);
            _loadedEnemies.Add(data.Load());
        }
        
        public static List<EnemyBase> Load(Vector3Int pChunk) {
            if (!_enemies.Remove(pChunk, out var enemies))
                return null;

            var result = new List<EnemyBase>();
            
            foreach (var enemy in enemies) {
                var newEnemy = enemy.Load();
                _loadedEnemies.Add(newEnemy);
                result.Add(newEnemy);
            }

            return result;
        }

        public static void UnLoad(Vector3Int pChunk) {
            Debug.Log($"Unloaded chunk: {pChunk}");
            var datas = _loadedEnemies.GroupBy(enemy => {
                
                var chunk = Chunk.GetChunkIdx(ChunkManager.Instance.Args, enemy.transform.position.ToVec3Int());
                return chunk.x == pChunk.x && chunk.z == pChunk.z;
            });

            var flag = false;
            foreach (var group in datas) {
                if (group.Key) {

                    _enemies.TryAdd(pChunk, new());
                    foreach (var enemy in group) {
                        _enemies[pChunk].Add(new(enemy));
                        Object.Destroy(enemy.gameObject);
                    }
                    continue;
                }

                flag = true;
                _loadedEnemies = group.ToList();
            }

            if (!flag)
                _loadedEnemies.Clear();

        }
    }
}