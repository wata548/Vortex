using System;
using System.Runtime.CompilerServices;
using Extension;
using FSM;
using MapGenerator;
using UnityEngine;

namespace Entity.FSM {
    public class AStarFollow: IState<EnemyState, EnemyBase> {

        private const float PROCEDURE_TIME_LIMIT = 4f;
        
        public EnemyState State { get; } = EnemyState.Follow;

        private readonly int _attackRange;
        private readonly int _detectRange;

        private float _speed;
        private float _procedureTime;
        public Vector3 _targetPos;
        private Vector3 _velocity;
        private Rigidbody _rigid = null;

        public AStarFollow(float pSpeed = 2, int pAttackRange = 1, int pDetectRange = 10) {
            _attackRange = pAttackRange;
            _detectRange = pDetectRange;
            _speed = pSpeed;
        }

        private Vector3 AStar(Vector3Int pStart, Vector3Int pDest) {
            var detectMapSize = _detectRange * 2 + 1;
            var visit = new bool[detectMapSize * detectMapSize * detectMapSize];
            var dist = new int[detectMapSize * detectMapSize * detectMapSize];
            var queue = new PriorityQueue<(Vector3Int Pos, Vector3 FirstMove, int F)>(true, 
                (lhs, rhs) => lhs.F.CompareTo(rhs.F)
            );
            
            queue.Enqueue((Vector3Int.zero, Vector3.zero, 1));
            
            var directions = new Vector3Int[] {
                Vector3Int.forward,
                Vector3Int.back,
                Vector3Int.left,
                Vector3Int.right,
            };

            while (queue.Count > 0) {
                var (pos, dir, _) = queue.Dequeue();
                var moveCnt =  GetDist(pos);
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
                            queue.Enqueue((nextPos, dir == Vector3.zero ? direction + Vector3Int.up : dir, heuristic + moveCnt + 1));
                            SetDist(nextPos, moveCnt + 1);
                        }
                    }
                }
            }

            throw new Exception("Not found");

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

        
        //==================================================||State 
        public void Update(EnemyBase pTarget) {

            if (ChunkManager.Instance.Player == null)
                return;
            
            _procedureTime += Time.deltaTime;
            var playerPos = ChunkManager.Instance.Player.transform.position;
            var dist = (playerPos - pTarget.transform.position).magnitude;
            
            if (dist <= _attackRange) {
                pTarget.FSM.Change(pTarget, EnemyState.Attack);
                return;
            }
            if (dist >= _detectRange) {
                pTarget.FSM.Change(pTarget, EnemyState.Idle);
                return;
            }

            var pos = pTarget.transform.position;
            if ((pos - _targetPos).magnitude <= 0.01f || _procedureTime >= PROCEDURE_TIME_LIMIT) {

                if(_targetPos != Vector3.zero)
                    pTarget.transform.position = _targetPos;
                var start = pTarget.transform.position.ToVec3Int();
                var dest = playerPos.ToVec3Int();
                _velocity = AStar(start, dest);
                _procedureTime = 0;
                _targetPos = pTarget.transform.position + _velocity;

                if (_velocity.y < 0)
                    _velocity.y = 0;
                
                _velocity.x *= _speed;
                _velocity.y *= Player.Movement.JUMP_SCALE;
                _velocity.z *= _speed;
            }
            _rigid ??= pTarget.GetComponent<Rigidbody>();
            var velocity = _rigid.velocity;
            (velocity.x, velocity.z) = (_velocity.x, _velocity.z);
            velocity.y += _velocity.y;
            
            _velocity.y = 0;
            
            _rigid.velocity = velocity;
        }

        public void Enter(EnemyBase pTarget, EnemyState pPrev) {
            Debug.Log($"Enter Follow mode({pTarget.name})");
            _procedureTime = PROCEDURE_TIME_LIMIT;
        }

        public void Exit(EnemyBase pTarget) {
            _rigid.velocity = Vector3.zero;
            Debug.Log($"Exit Follow mode({pTarget.name})");
        }
    }
}