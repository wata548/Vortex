using System;
using System.Runtime.CompilerServices;
using Extension;
using FSM;
using MapGenerator;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Enemy.FSM {
    
    [Serializable]
    public class AStarFollow: IState<EnemyState, EnemyBase> {

        //==================================================||Constant 
        private const float PROCEDURE_TIME_LIMIT = 2f;
        
        //==================================================|| Properties 
        public EnemyState State { get; } = EnemyState.Follow;

        #if UNITY_EDITOR
        public Vector3 TargetPos => _targetPos; 
        #endif
        
        //==================================================|| Fields
        private readonly int _attackRange;
        private readonly int _detectRange;

        private float _procedureTime;
        private Vector3 _targetPos;
        
        private GroundMovement _movement = null;
        
        //==================================================|| Constructors
        public AStarFollow(int pAttackRange = 1, int pDetectRange = 10) {
            _attackRange = pAttackRange;
            _detectRange = pDetectRange;
        }

        //==================================================||Methods 
        private Vector3Int AStar(Vector3Int pStart, Vector3Int pDest) {
            var detectMapSize = _detectRange * 2 + 1;
            var visit = new bool[detectMapSize * detectMapSize * detectMapSize];
            var dist = new int[detectMapSize * detectMapSize * detectMapSize];
            var queue = new PriorityQueue<(Vector3Int Pos, Vector3Int FirstMove, int Value)>(true, 
                (lhs, rhs) => lhs.Value.CompareTo(rhs.Value)
            );
            
            queue.Enqueue((Vector3Int.zero, Vector3Int.zero, 0));
            
            //this function assume that the two points are in contact with ground  
            while(EnemyBase.IsAir(pDest + Vector3Int.down))
                pDest += Vector3Int.down;
            while(EnemyBase.IsAir(pStart + Vector3Int.down))
                pStart += Vector3Int.down;
            
            if(pDest == pStart)
                return Vector3Int.zero;
            
            while (queue.Count > 0) {
                
                var (pos, dir, _) = queue.Dequeue();
                var moveCnt =  GetDist(pos);
                
                if(GetVisit(pos))
                    continue;
                SetVisit(pos, true);
                
                foreach (var direction in EnemyBase.DIRECTIONS) {

                    if(pos.x + direction.x > _detectRange || pos.x + direction.x < -_detectRange)
                        continue;
                    if(pos.y + direction.y > _detectRange || pos.y + direction.y < -_detectRange)
                        continue;
                    if(pos.z + direction.z > _detectRange || pos.z + direction.z < -_detectRange)
                        continue;

                    if (GetVisit(pos + direction)) {
                        continue;
                    }

                    if (pStart + pos + direction == pDest) {
                        return dir;
                    }
                    
                    if (!EnemyBase.IsAir(pStart + pos + direction + Vector3Int.up)) {
                        continue;
                    }
                    //down or straight
                    if (EnemyBase.IsAir(pStart + pos + direction)) {
                        var nextPos = pos + direction;
                        var gravity = Vector3Int.zero;
                        
                        while (EnemyBase.IsAir(pStart + (nextPos + gravity + Vector3Int.down)))
                            gravity += Vector3Int.down;
                            
                        
                        var heuristic = (pStart + nextPos - pDest).sqrMagnitude;
                        var prevValue = GetDist(nextPos);

                        nextPos += gravity; 
                        if (!GetVisit(nextPos) && (prevValue == 0 || prevValue > moveCnt + 1)) {
                            if (pStart + nextPos == pDest)
                                return dir == Vector3Int.zero ? direction + gravity : dir;
                            
                            queue.Enqueue((nextPos, dir == Vector3Int.zero ? direction + gravity : dir, heuristic + moveCnt + 1));
                            SetDist(nextPos, moveCnt + 1);    
                        }
                        
                    }
                    //jump
                    else if (EnemyBase.IsAir(pStart + pos + direction + 2 * Vector3Int.up)
                             && EnemyBase.IsAir(pStart + pos + 2 * Vector3Int.up)) 
                    {
                        var nextPos = pos + direction + Vector3Int.up;
                        var heuristic = (pStart + nextPos - pDest).sqrMagnitude;
                        var prevValue = GetDist(nextPos);
                        
                        if (!GetVisit(nextPos) && (prevValue == 0 || prevValue > moveCnt + 1)) {
                            if (pStart + nextPos == pDest)
                                return dir == Vector3Int.zero ? direction + Vector3Int.up: dir;
                            
                            queue.Enqueue((nextPos, dir == Vector3Int.zero ? direction + Vector3Int.up : dir, heuristic + moveCnt + 1));
                            SetDist(nextPos, moveCnt + 1);
                        }
                    }
                }
            }

            Debug.LogWarning("can't find player");
            return Vector3Int.zero;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool GetVisit(Vector3Int pPos) =>
                visit[detectMapSize * (detectMapSize * (pPos.x + _detectRange) + pPos.y + _detectRange) + pPos.z + _detectRange];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool SetVisit(Vector3Int pPos, bool pValue) =>
                visit[detectMapSize * (detectMapSize * (pPos.x + _detectRange) + pPos.y + _detectRange) + pPos.z + _detectRange] = pValue;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int GetDist(Vector3Int pPos) =>
                dist[detectMapSize * (detectMapSize * (pPos.x + _detectRange) + pPos.y + _detectRange) + pPos.z + _detectRange];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int SetDist(Vector3Int pPos, int pValue) =>
                dist[detectMapSize * (detectMapSize * (pPos.x + _detectRange) + pPos.y + _detectRange) + pPos.z + _detectRange] = pValue;
        }

        //==================================================||Main Logic 

        public void Update(EnemyBase pTarget) {

            if (ChunkManager.Instance.Player == null)
                return;
            
            _procedureTime -= Time.deltaTime;
            var playerPos = ChunkManager.Instance.Player.transform.position;
            var dist = (playerPos - pTarget.transform.position);

            if (dist.magnitude - pTarget.transform.localScale.z / 2 <= _attackRange) {
                pTarget.FSM.Change(pTarget, EnemyState.Attack);
                return;
            }
            if (Mathf.Abs(dist.x) >= _detectRange || Mathf.Abs(dist.y) >= _detectRange || Mathf.Abs(dist.z) >= _detectRange) {
                pTarget.FSM.Change(pTarget, EnemyState.Idle);
                return;
            }

            var diff = _targetPos - pTarget.transform.position;
            diff.y = 0;
            
            if (diff.magnitude <= 0.1f || _procedureTime <= 0) {

                _targetPos = pTarget.FixedPos;
                pTarget.transform.position = _targetPos;
                
                var start = pTarget.FootPos;
                var dest = playerPos.ToVec3Int();
                var delta = AStar(start, dest);
                
                if (delta == Vector3.zero) {
                    
                    pTarget.FSM.Change(pTarget, EnemyState.Attack);
                    return;
                }
                
                _procedureTime = PROCEDURE_TIME_LIMIT;
                _targetPos += delta;

                if (delta.y > 0)
                    _movement.Jump();
                delta.y = 0;
                
                _movement.SetDirection(delta);
            }
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            Debug.Log($"Enter Follow mode({pTarget.name})");
            _movement = pTarget.GetComponent<GroundMovement>();
            
            if (_movement == null)
                throw new Exception($"Enemy must have ground movement component. ({pTarget.name})");
            
            _targetPos = pTarget.FixedPos;
            pTarget.transform.position = _targetPos;
            
            _procedureTime = PROCEDURE_TIME_LIMIT;
        }

        public void Exit(EnemyBase pTarget) {
            Debug.Log($"Exit Follow mode({pTarget.name})");
            _movement?.SetDirection(Vector3.zero);
            _targetPos = Vector3.zero;
        }
    }
}