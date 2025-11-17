using MapGenerator;
using Player;

namespace Entity.Enemy.FSM {
    public class RangeFinder: IFindPlayer<EnemyBase> {

        private float _range; 
        
        public RangeFinder(float pRange) => _range = pRange;
        
        public bool PlayerExist(EnemyBase pTarget) {
            return (PlayerEntity.Instance.transform.position - pTarget.transform.position).magnitude <= _range;
        }
    }
}