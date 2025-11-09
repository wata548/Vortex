using Player;
using UnityEngine;

namespace Entity.Enemy {
    public class GroundMovement: Movement {

        private readonly EnemyInputSetting _input = new();
        protected override IInputSetting _inputSetting => _input;

        public void Jump() => _input.Jump();
        
        public void SetDirection(Vector3 pDir) {

            if (pDir == Vector3.zero) {
                _input.SetDirection(Vector3.zero);
                return;
            }
                
            
            pDir.y = 0;
            pDir = pDir.normalized;

            var degree = Mathf.Atan2(pDir.x, pDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,degree, 0);
            _input.SetDirection(Vector3.forward);
        } 
    }
}