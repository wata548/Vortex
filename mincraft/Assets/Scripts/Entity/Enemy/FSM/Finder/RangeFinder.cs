using MapGenerator;

namespace Entity.Enemy.FSM {
    public class RangeFinder: IFindPlayer<EnemyBase> {

        private float _range; 
        
        public RangeFinder(float pRange) => _range = pRange;
        
        public bool PlayerExist(EnemyBase pTarget) {
            return (ChunkManager.Instance.Player.transform.position - pTarget.transform.position).magnitude <= _range;
        }
    }
}