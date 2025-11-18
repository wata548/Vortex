using Entity;
using Extension;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player {
    
    [CreateAssetMenu(menuName = "Input/Key")]
    public class KeyBaseScriptableInputSetting: ScriptableInputSetting {
        [SerializeField] private KeyCode _left = KeyCode.A;
        [SerializeField] private KeyCode _right = KeyCode.D;
        [SerializeField] private KeyCode _front = KeyCode.W;
        [SerializeField] private KeyCode _back = KeyCode.S;
        [SerializeField] private KeyCode _jump = KeyCode.Space;
        [SerializeField] private KeyCode _breakBlock = KeyCode.Mouse0;
        [SerializeField] private KeyCode _placeBlock = KeyCode.Mouse1;
        [SerializeField] private KeyCode _menu = KeyCode.Escape;
        [SerializeField] private string _cameraYaw = "Mouse X";
        [SerializeField] private string _cameraPitch = "Mouse Y";

        public override bool IsJumpStart =>
            Input.GetKeyDown(_jump);

        public override Vector2 CameraDirection =>
            new(Input.GetAxisRaw(_cameraYaw), -Input.GetAxisRaw(_cameraPitch));

        public override Vector3 InputDirection{
            get {
                var result = Vector3.zero;
                if(Input.GetKey(_left))
                    result += Vector3.left;
                if(Input.GetKey(_right))
                    result += Vector3.right;
                if(Input.GetKey(_front))
                    result += Vector3.forward;
                if(Input.GetKey(_back))
                    result += Vector3.back;

                return result.normalized;
            }
        }
        public override bool BreakBlock => Input.GetKey(_breakBlock);
        public override bool PlaceBlock => Input.GetKeyDown(_placeBlock);

        public override int SelectItemSlot => -(int)Input.mouseScrollDelta.y.Sign();
        public override bool Menu => Input.GetKeyDown(_menu);
    }
}