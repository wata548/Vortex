using System;
using System.Collections.Generic;
using Entity.FSM;
using Extension;
using FSM;
using UnityEngine;

namespace Entity {
    
    public abstract class EnemyBase: MonoBehaviour, IEntity {
        
        //==================================================||Properties 
        public bool IsAlive { get; private set; } = true;
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public float Speed { get; protected set; }
        public EnemyFsm FSM { get; private set; }

        //==================================================||Methods 
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