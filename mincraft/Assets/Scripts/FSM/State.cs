using System;
using UnityEngine;

namespace FSM {
    public interface IState<TKey, TTarget> 
        where TKey: Enum
        where TTarget: class {
        public TKey State { get; }
        
        void Update(TTarget pTarget);
        
        void Enter(TTarget pTarget, TKey pPrev);
        void Exit(TTarget pTarget);
    }

    public class LogState<TKey, TTarget>: IState<TKey, TTarget>
        where TKey : Enum
        where TTarget : class {

        public TKey State { get; }
        
        public LogState(TKey pState) {
            State = pState;
        }
        
        public void Update(TTarget pTarget) {}

        public void Enter(TTarget pTarget, TKey pPrev) {
            Debug.Log($"Enter: {pPrev} -> {State}");
        }

        public void Exit(TTarget pTarget) {}
    }
}