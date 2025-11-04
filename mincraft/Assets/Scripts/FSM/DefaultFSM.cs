using System;
using JetBrains.Annotations;
using UnityEngine;

namespace FSM {
    
    public class DefaultFSM<TKey, TTarget> 
        where TKey: Enum
        where TTarget: class {
 
        
        //==================================================||Properties 
        public TKey CurState => _stateBehaviour != null
            ? _stateBehaviour.State
            : default;
        
        //==================================================||Fields
        protected IState<TKey, TTarget> _stateBehaviour = null;

       //==================================================||Constructors
        //public DefaultFSM(IState<TKey, TTarget> pState) =>
        //    _state = pState;

       //==================================================||Methods 
        public void Change(TTarget pTarget, IState<TKey, TTarget> pState) {
            _stateBehaviour?.Exit(pTarget);
            _stateBehaviour = pState;
            _stateBehaviour.Enter(pTarget, CurState);
        } 
        
        public void Update(TTarget pTarget) {
            _stateBehaviour?.Update(pTarget);
        }
    }
}