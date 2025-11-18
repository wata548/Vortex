using Entity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player {
    [CreateAssetMenu(menuName = "Input/Axis")]
    public class AxisBaseScriptableInputSetting: ScriptableInputSetting {

        [SerializeField] private string _moveForward = "Horizontal1";
        [SerializeField] private string _moveSide = "Vertical1";
        [SerializeField] private string _cameraYaw = "Horizontal2";
        [SerializeField] private string _cameraPitch = "Vertical2";
        [SerializeField] private KeyCode _jump = KeyCode.JoystickButton0;
        [SerializeField] private KeyCode _breakBlock = KeyCode.JoystickButton5;
        [SerializeField] private KeyCode _placeBlock = KeyCode.JoystickButton4;
        [SerializeField] private KeyCode _leftItemSlot = KeyCode.JoystickButton2;
        [SerializeField] private KeyCode _rightItemSlot = KeyCode.JoystickButton1;
        [SerializeField] private KeyCode _menu = KeyCode.JoystickButton6;

       //==================================================||Movement 
        public override Vector3 InputDirection =>
            new(Input.GetAxisRaw(_moveForward), 0, Input.GetAxisRaw(_moveSide));

        public override bool IsJumpStart => Input.GetKeyDown(_jump);
        
       //==================================================||Camera 
        public override Vector2 CameraDirection =>
            new(Input.GetAxisRaw(_cameraYaw), -Input.GetAxisRaw(_cameraPitch));

       //==================================================||Player 
        public override bool BreakBlock => Input.GetKey(_breakBlock);
        public override bool PlaceBlock => Input.GetKeyDown(_placeBlock);

        public override int SelectItemSlot {
            get {
                int result = 0;
                if (Input.GetKeyDown(_leftItemSlot))
                    result--;
                if (Input.GetKeyDown(_rightItemSlot))
                    result++;
                return result;
            }
        }

        public override bool Menu => Input.GetKeyDown(_menu);
    }
}