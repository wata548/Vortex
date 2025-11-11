using System;
using System.Collections.Generic;
using Entity.Enemy.FSM;
using Extension;
using Extension.Test;
using FSM;
using MapGenerator;
using UnityEngine;

namespace Entity.Enemy {
    
    public abstract class EnemyBase: MonoBehaviour, IEntity {

        public static readonly Vector3Int[] DIRECTIONS = new[] {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right,
        };
        
        //==================================================||Properties 
        public bool IsAlive { get; private set; } = true;
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public float Speed { get; protected set; }
        public EnemyFsm FSM { get; private set; }

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
        
        //==================================================||Methods 
        [TestMethod] public void ShowState() => Debug.Log(FSM.CurState);
        
        public void SetUp(EnemyData pData) {
            IsAlive = pData.IsAlive;
            MaxHp = pData.MaxHp;
            Hp = pData.Hp;
            Speed = pData.Speed;
            transform.position = pData.FixedPos;
        }
        
        
        //Check this position's block equal to air.
        //But if this position that isn't loaded, it out false
        public static bool IsAir(Vector3Int pPos) {
            if (!ChunkManager.Instance.IsLoadedChunk(pPos))
                return false;
            return ChunkManager.Instance.GetMapData(pPos) == Block.Air;
        }
        protected abstract Dictionary<EnemyState, IState<EnemyState, EnemyBase>> RegisterEnemyStateMap();
       
        protected virtual void OnDamage(int pAmount){}
        protected virtual void OnDeath(){}
        protected virtual void OnHeal(int pAmount){}
        
        public void GetDamage(int pAmount) {
            Hp -= pAmount;
            OnDamage(pAmount);
            
            if (Hp < 0) {
                IsAlive = false;
                Hp = 0;
                OnDeath();
            }

        }

        public void Heal(int pAmount) {
            Hp = Math.Max(MaxHp, Hp + pAmount);
            OnHeal(pAmount);
        }
        
        //==================================================||Unity 
        protected void Awake() {
            FSM = new(RegisterEnemyStateMap());
        }

        protected void Update() {
            FSM.Update(this);
        }

        protected void OnDrawGizmos() {
            var a = (transform.position).ToVec3Int() + Vector3.one * 0.5f;
            Gizmos.DrawCube(a, Vector3.one);
        }
    }
}