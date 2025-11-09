using System;
using JetBrains.Annotations;
using UnityEngine;

namespace FSM {
    
    public abstract class DefaultFSM<TKey, TTarget> 
        where TKey: Enum
        where TTarget: class {
 
        
        //==================================================||Properties 
        public TKey CurState => _stateBehaviour != null
            ? _stateBehaviour.State
            : default;
        
        //==================================================||Fields
        protected IState<TKey, TTarget> _stateBehaviour = null;

       //==================================================||Methods 
        public void Change(TTarget pTarget, IState<TKey, TTarget> pState) {
            _stateBehaviour?.Exit(pTarget);
            var curState = CurState;
            _stateBehaviour = pState;
            _stateBehaviour.Enter(pTarget, curState);
        } 
        
        public void Update(TTarget pTarget) {
            _stateBehaviour?.Update(pTarget);
        }
    }
}