using System.Collections.Generic;
using FSM;

namespace Entity.Enemy.FSM {
    public class EnemyFsm: DefaultFSM<EnemyState, EnemyBase> {

        private IReadOnlyDictionary<EnemyState, IState<EnemyState, EnemyBase>> _stateMap;
        
        public EnemyFsm(Dictionary<EnemyState, IState<EnemyState, EnemyBase>> pStateMap, EnemyState pState = default) {
            _stateMap = pStateMap;
            _stateBehaviour = _stateMap[pState];
        }

        public void Change(EnemyBase pThis, EnemyState pState) {
            Change(pThis, _stateMap[pState]);
        }
    }
}