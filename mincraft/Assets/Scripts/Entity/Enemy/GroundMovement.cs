using Player;
using UnityEngine;

namespace Entity.Enemy {
    public class GroundMovement: Movement {

        private readonly EnemyInputSetting _input = new();
        protected override IInputSetting _inputSetting => _input;

        public void Jump() => _input.Jump();
        
        public void SetDirection(Vector3 pDir) {

            pDir.y = 0;
            pDir = pDir.normalized;
            _input.SetDirection(pDir);
        } 
    }
}