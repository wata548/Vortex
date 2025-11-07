using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Entity.Enemy {
    public class EnemyInputSetting: IInputSetting {

        private bool _isJumping = false;
        
        public void Jump() {
            _isJumping = true;
        }
        public void SetDirection(Vector3 pDir) {
            InputDirection = pDir;
        }
        
        public Vector3 InputDirection { get; private set; }

        public bool IsJumpStart {
            get {
                var temp = _isJumping;
                _isJumping = false;
                return temp;
            }
        }
    }
}