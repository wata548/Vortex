using Extension;
using FSM;
using MapGenerator;
using UnityEngine;

namespace Entity.Enemy.FSM {
    public class RandomPatrol: IState<EnemyState, EnemyBase> {

        //==================================================||Constant 
        public EnemyState State { get; } = EnemyState.Patrol;
        private const float PROCEDURE_TIME_LIMIT = 4f;
        
       //==================================================||Fields 
        private IFindPlayer<EnemyBase> _finder;
        private int _min;
        private int _max;
        private float _procedureTime;
        
        private int _remainCnt;
        private Vector3 _targetPos;
        private Vector3Int _dir;

        private GroundMovement _movement;
        
       //==================================================||Constructor 
        public RandomPatrol(IFindPlayer<EnemyBase> pFinder, int pMin = 1, int pMax = 3) {
            _max = pMax;
            _min = pMin;
            _finder = pFinder;
        }
        
        //==================================================||Methods 
        private bool AbleToMove(Vector3Int pPos) {
            //Blocked
            if (!EnemyBase.IsAir(pPos + _dir + Vector3Int.up))
                return false;
            
            //Straight
            if (EnemyBase.IsAir(pPos + _dir))
                return true;
            
            //Jump
            if (EnemyBase.IsAir(pPos + 2 * Vector3Int.up) 
                && EnemyBase.IsAir(pPos + _dir + 2 * Vector3Int.up)) 
            {
                _movement.Jump();
                return true;
            }

            return false;
        }

        private bool Move(EnemyBase pTarget) {
            
            _procedureTime -= Time.deltaTime;
            var diff = pTarget.transform.position - _targetPos;
            diff.y = 0;

            if (diff.magnitude > 0.01f && _procedureTime > 0)
                return false;
            
            _procedureTime = PROCEDURE_TIME_LIMIT;
            
            _targetPos = pTarget.FixedPos;
            pTarget.transform.position = _targetPos;
                            
            _targetPos += _dir;
            _remainCnt--;
            
            _movement.SetDirection(_dir);
            return true;
        }
        
        //==================================================||MainLogic 
       
        public void Update(EnemyBase pTarget) {
            
            if (_finder.PlayerExist(pTarget)) {
                pTarget.FSM.Change(pTarget, EnemyState.Follow);
                return;
            }

            if (!Move(pTarget))
                return;
            
            if (_remainCnt == 0) {
                pTarget.FSM.Change(pTarget, EnemyState.Idle);
                return;
            }
            if (!AbleToMove(pTarget.FootPos)) {
                pTarget.FSM.Change(pTarget, EnemyState.Idle);
            }
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            Debug.Log("Enter Patrol");
            
            _procedureTime = 0;
            _movement = pTarget.GetComponent<GroundMovement>();
            
            _remainCnt = Random.Range(_min, _max + 1) + 1;
            _dir = EnemyBase.DIRECTIONS[Random.Range(0, EnemyBase.DIRECTIONS.Length)];

            _targetPos = pTarget.FixedPos;
            pTarget.transform.position = _targetPos;
            Debug.Log($"Next: {_targetPos}");
        }

        public void Exit(EnemyBase pTarget) {
            _movement?.SetDirection(Vector3.zero);
        }
    }
}