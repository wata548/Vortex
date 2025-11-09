using System;
using System.Runtime.CompilerServices;
using Extension;
using FSM;
using MapGenerator;
using Unity.VisualScripting;
using UnityEngine;

namespace Entity.Enemy.FSM {
    
    [Serializable]
    public class AStarFollow: IState<EnemyState, EnemyBase> {

        //==================================================||Constant 
        private const float PROCEDURE_TIME_LIMIT = 4f;
        
        //==================================================|| Properties 
        public EnemyState State { get; } = EnemyState.Follow;

        //==================================================|| Fields
        private readonly int _attackRange;
        private readonly int _detectRange;

        private float _procedureTime;
        public Vector3 _targetPos;
        public Vector3 _velocity;
        
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
            var queue = new PriorityQueue<(Vector3Int Pos, Vector3Int FirstMove, int F)>(true, 
                (lhs, rhs) => lhs.F.CompareTo(rhs.F)
            );
            
            queue.Enqueue((Vector3Int.zero, Vector3Int.zero, 1));
            
            var directions = new Vector3Int[] {
                Vector3Int.forward,
                Vector3Int.back,
                Vector3Int.left,
                Vector3Int.right,
            };

            while(ChunkManager.Instance.GetMapData(pDest + Vector3Int.down) == Block.Air)
                pDest += Vector3Int.down;
            while(ChunkManager.Instance.GetMapData(pStart + Vector3Int.down) == Block.Air)
                pStart += Vector3Int.down;
            
            if(pDest == pStart)
                return Vector3Int.zero;
            
            while (queue.Count > 0) {
                var (pos, dir, _) = queue.Dequeue();
                var moveCnt =  GetDist(pos);
                if(GetVisit(pos))
                    continue;
                SetVisit(pos, true);
                
                foreach (var direction in directions) {

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
                    
                    if (ChunkManager.Instance.GetMapData(pStart + pos + direction + Vector3Int.up) != Block.Air) {
                        continue;
                    }
                    //down or straight
                    if (ChunkManager.Instance.GetMapData(pStart + pos + direction) == Block.Air) {
                        var nextPos = pos + direction;
                        var gravity = Vector3Int.zero;
                        
                        while (ChunkManager.Instance.GetMapData(pStart + (nextPos + gravity + Vector3Int.down)) == Block.Air)
                            gravity += Vector3Int.down;
                            
                        
                        var heuristic = (pStart + nextPos - pDest).sqrMagnitude;
                        var prevValue = GetDist(nextPos);

                        nextPos += gravity; 
                        if (!GetVisit(nextPos) && (prevValue == 0 || GetDist(nextPos) > moveCnt + 1)) {
                            if (pStart + nextPos == pDest)
                                return dir == Vector3.zero ? direction + gravity : dir;
                            queue.Enqueue((nextPos, dir == Vector3.zero ? direction + gravity : dir, heuristic + moveCnt + 1));
                            SetDist(nextPos, moveCnt + 1);    
                        }
                        
                    }
                    //up
                    else if (ChunkManager.Instance.GetMapData(pStart + pos + direction + 2 * Vector3Int.up) == Block.Air 
                             && ChunkManager.Instance.GetMapData(pStart + pos + 2 * Vector3Int.up) == Block.Air) 
                    {
                        var nextPos = pos + direction + Vector3Int.up;
                        var heuristic = (pStart + nextPos - pDest).sqrMagnitude;
                        var prevValue = GetDist(nextPos);
                        if (!GetVisit(nextPos) && (prevValue == 0 || prevValue > moveCnt + 1)) {
                            if (pStart + nextPos == pDest)
                                return dir == Vector3.zero ? direction + Vector3Int.up: dir;
                            queue.Enqueue((nextPos, dir == Vector3.zero ? direction + Vector3Int.up : dir, heuristic + moveCnt + 1));
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

        private Vector3 _dist; 
        public void Update(EnemyBase pTarget) {

            if (ChunkManager.Instance.Player == null)
                return;
            
            _procedureTime += Time.deltaTime;
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

            var pos = pTarget.transform.position;
            var dir = _targetPos - pos;
            dir.y = 0;
            
            if (dir.magnitude <= 0.1f || _procedureTime >= PROCEDURE_TIME_LIMIT) {

                _targetPos = pTarget.transform.position.ToVec3Int() + Vector3.one * 0.5f;
                var temp = _targetPos;
                temp.y = pTarget.transform.position.y;
                
                pTarget.transform.position = temp;
                
                var start = pTarget.Pos;
                var dest = playerPos.ToVec3Int();
                _velocity = AStar(start, dest);
                if (_velocity == Vector3.zero) {
                    
                    pTarget.FSM.Change(pTarget, EnemyState.Attack);
                    return;
                }
                
                _procedureTime = 0;
                _targetPos += _velocity;

                if (_velocity.y < 0)
                    _velocity.y = 0;
                
                if (_velocity.y > 0)
                    _movement.Jump();
                _movement.SetDirection(_velocity);
            }
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            Debug.Log($"Enter Follow mode({pTarget.name})");
            _movement = pTarget.GetComponent<GroundMovement>();
            if (_movement == null)
                throw new Exception($"Enemy must have ground movement component. ({pTarget.name})");
            
            //grid

            var targetPos = pTarget.transform.position;
            pTarget.transform.position = pTarget.Pos + new Vector3(0.5f, pTarget.transform.localScale.y * 0.5f, 0.5f);
            
            _targetPos = targetPos.ToVec3Int() + Vector3.one * 0.5f;
            _procedureTime = PROCEDURE_TIME_LIMIT;
        }

        public void Exit(EnemyBase pTarget) {
            Debug.Log($"Exit Follow mode({pTarget.name})");
            _movement?.SetDirection(Vector3.zero);
            _targetPos = Vector3.zero;
        }
    }
}