using System;
using Entity;
using Extension.Test;
using MapGenerator;
using UnityEngine;

namespace Player {
    
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(Interaction))]
    public class Player: MonoBehaviour, IEntity {
        
        public PlayerMovement Movement { get; private set; }
        public bool IsAlive { get; }
        public int MaxHp { get; }
        public int Hp { get; }
        public float Speed { get; }
        
        public Vector3 FixedPos => new(
            Mathf.Floor(transform.position.x) + 0.5f,
            transform.position.y,
            Mathf.Floor(transform.position.z) + 0.5f
        );
        
        public Vector3Int FootPos => new(
            Mathf.FloorToInt(transform.position.x),
            //floating point error
            Mathf.RoundToInt(transform.position.y - transform.localScale.y * 0.5f),
            Mathf.FloorToInt(transform.position.z)
        );

        [TestMethod]
        private void CurPos() {
            var pos = Chunk.GetChunkPos(ChunkManager.Instance.Args, transform.position, out var idx);
            Debug.Log($"Player: {idx} - {pos}");
        }
        
        public void GetDamage(int pAmount) {
            throw new System.NotImplementedException();
        }

        public void Heal(int pAmount) {
            throw new System.NotImplementedException();
        }

        private void Awake() {
            Movement = GetComponent<PlayerMovement>();
        }
    }
}