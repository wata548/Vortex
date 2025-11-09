using FSM;
using UnityEngine;

namespace Entity.Enemy.FSM {
    public class RandomIdle: IState<EnemyState, EnemyBase> {

       //==================================================||Constants 
        public EnemyState State { get; } = EnemyState.Idle;
        
       //==================================================||Fields 
        private IFindPlayer<EnemyBase> _finder;
        private float _min;
        private float _max;
        private float _remain;
        private EnemyState[] _next;

       //==================================================||Constructor 
        public RandomIdle(IFindPlayer<EnemyBase> pFinder, float pMin = 0.2f, float pMax = 0.5f, EnemyState[] pNext = null) {
            _max = pMax;
            _min = pMin;
            _finder = pFinder;
            _next = pNext ?? new[] { EnemyState.Patrol };
        }
        
        //==================================================||MainLogic 
       
        public void Update(EnemyBase pTarget) {
            
            if (_finder.PlayerExist(pTarget)) {
                pTarget.FSM.Change(pTarget, EnemyState.Follow);
                return;
            }

            _remain -= Time.deltaTime;
            if (_remain > 0) {
                var idx = Random.Range(0, _next.Length);
                pTarget.FSM.Change(pTarget, _next[idx]);
            }
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            _remain = Random.Range(_min, _max);
            Debug.Log($"Enter Idle (Wait: {_remain}");
        }

        public void Exit(EnemyBase pTarget) {
        }
    }
}