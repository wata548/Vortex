using System.Collections.Generic;
using Entity.Enemy.FSM;
using Extension.Test;
using FSM;
using UnityEngine;

namespace Entity.Enemy {
    
    [RequireComponent(typeof(GroundMovement))]
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
            
            _follow = new AStarFollow(_attackRange, _detectRange);
            var finder = new RangeFinder(7);
            
            return new() {
            
                { EnemyState.Idle, new RandomIdle(finder, 1f, 2f, pNext: new[] { EnemyState.Patrol }) },
                { EnemyState.Attack, new LogState<EnemyState, EnemyBase>(EnemyState.Attack) },
                { EnemyState.Patrol, new RandomPatrol(finder) },
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