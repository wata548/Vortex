using Entity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player {
    [CreateAssetMenu(menuName = "Input/Axis")]
    public class AxisBaseScriptableInputSetting: ScriptableInputSetting {

        [SerializeField] private string _moveForward;
        [SerializeField] private string _moveSide;
        [SerializeField] private string _cameraYaw;
        [SerializeField] private string _cameraPitch;
        [SerializeField] private KeyCode _jump = KeyCode.JoystickButton0;
        [SerializeField] private KeyCode _breakBlock = KeyCode.JoystickButton1;
        [SerializeField] private KeyCode _menu = KeyCode.JoystickButton3;

        public override Vector3 InputDirection =>
            new(Input.GetAxisRaw(_moveForward), 0, Input.GetAxisRaw(_moveSide));

        public override bool IsJumpStart => Input.GetKeyDown(_jump);
        public override Vector2 CameraDirection =>
            new(Input.GetAxisRaw(_cameraYaw), -Input.GetAxisRaw(_cameraPitch));

        public override bool BreakBlock => Input.GetKey(_breakBlock);
        public override bool Menu => Input.GetKeyDown(_menu);
    }
}