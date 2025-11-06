using System;
using System.Collections.Generic;
using Entity.FSM;
using Extension.Test;
using FSM;
using Unity.VisualScripting;
using UnityEngine;

namespace Entity {
    public class GroundEnemyBase: EnemyBase {

        [SerializeField] private int _detectRange = 15; 
        [SerializeField] private int _attackRange = 1; 
        [SerializeField] private int _speed = 2;
        private AStarFollow _follow;
        
        [TestMethod]
        private void Test() {
            FSM.Change(this, EnemyState.Follow);
        }

        protected override Dictionary<EnemyState, IState<EnemyState, EnemyBase>> RegisterEnemyStateMap() {
            
            _follow = new AStarFollow(_speed, _attackRange, _detectRange);
            return new() {
            
                { EnemyState.Idle, new LogState<EnemyState, EnemyBase>(EnemyState.Idle) },
                { EnemyState.Attack, new LogState<EnemyState, EnemyBase>(EnemyState.Attack) },
                { EnemyState.Follow,  _follow},
            };
        
        }

        protected new void OnDrawGizmos() {

            base.OnDrawGizmos();
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(_follow?._targetPos ?? Vector3.zero, Vector3.one);
        }
    }
}