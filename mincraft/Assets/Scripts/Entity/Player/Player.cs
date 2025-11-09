using Entity;
using UnityEngine;

namespace Player {
    
    [RequireComponent(typeof(PlayerMovement))]
    public class Player: MonoBehaviour, IEntity {
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
        public void GetDamage(int pAmount) {
            throw new System.NotImplementedException();
        }

        public void Heal(int pAmount) {
            throw new System.NotImplementedException();
        }
    }
}