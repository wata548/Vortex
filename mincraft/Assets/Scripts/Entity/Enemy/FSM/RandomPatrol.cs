using Extension;
using FSM;
using MapGenerator;
using UnityEngine;

namespace Entity.Enemy.FSM {
    public class RandomPatrol: IState<EnemyState, EnemyBase> {

        //==================================================||Constant 
        private const float PROCEDURE_TIME_LIMIT = 4f;
        
       //==================================================||Fields 
        public EnemyState State { get; } = EnemyState.Patrol;
        private IFindPlayer<EnemyBase> _finder;
        private int _min;
        private int _max;
        private float _procedureTime;
        
        private int _remainCnt;
        private Vector3 _targetPos;
        private Vector3 _dir;

        private GroundMovement _movement;
        
       //==================================================||Constructor 
        public RandomPatrol(IFindPlayer<EnemyBase> pFinder, int pMin = 1, int pMax = 3) {
            _max = pMax;
            _min = pMin;
            _finder = pFinder;
        }

        private bool AbleToMove(Vector3Int pPos) {
            if (ChunkManager.Instance.GetMapData(pPos + _dir + Vector3.up) != Block.Air)
                return false;
            if (ChunkManager.Instance.GetMapData(pPos + _dir) == Block.Air)
                return true;
            
            if (ChunkManager.Instance.GetMapData(pPos + 2 * Vector3Int.up) == Block.Air &&
                ChunkManager.Instance.GetMapData(pPos + _dir + 2 * Vector3.up) == Block.Air) 
            {
                _movement.Jump();
                return true;
            }

            return false;
        }
        
        //==================================================||MainLogic 
       
        public void Update(EnemyBase pTarget) {
            if (_finder.PlayerExist(pTarget)) {
                pTarget.FSM.Change(pTarget, EnemyState.Follow);
                return;
            }

            _procedureTime -= Time.deltaTime;
            
            var diff = pTarget.transform.position - _targetPos;
            diff.y = 0;
            if (diff.magnitude <= 0.01f || _procedureTime <= 0) {
                _procedureTime = PROCEDURE_TIME_LIMIT;
                
                _targetPos = pTarget.transform.position.ToVec3Int() + Vector3.one * 0.5f;
                var temp = _targetPos;
                temp.y = pTarget.transform.position.y;
                
                pTarget.transform.position = temp;
                
                _targetPos += _dir;
                _remainCnt--;
                if (_remainCnt == 0) {
                    pTarget.FSM.Change(pTarget, EnemyState.Idle);
                }
                
                if (!AbleToMove(pTarget.Pos)) {
                    pTarget.FSM.Change(pTarget, EnemyState.Idle);
                    return;
                }
                _movement.SetDirection(_dir);
            }
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            Debug.Log("Enter Patrol");
            
            _procedureTime = PROCEDURE_TIME_LIMIT;
            
            var directions = new Vector3Int[] {
                Vector3Int.forward,
                Vector3Int.back,
                Vector3Int.left,
                Vector3Int.right,
            };

            _movement = pTarget.GetComponent<GroundMovement>();
            
            _remainCnt = Random.Range(_min, _max + 1) + 1;
            _dir = directions[Random.Range(0, directions.Length)];

            _targetPos = pTarget.Pos + new Vector3(0.5f, pTarget.transform.localScale.y * 0.5f, 0.5f);
            pTarget.transform.position = _targetPos;
            Debug.Log(_targetPos);

        }

        public void Exit(EnemyBase pTarget) {
            _movement?.SetDirection(Vector3.zero);
        }
    }
}