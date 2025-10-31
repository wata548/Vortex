using UnityEngine;

namespace MapGenerator.Player {
    
    [CreateAssetMenu(menuName = "Input")]
    public class DefaultScriptableInputSetting: ScriptableInputSetting {
        [SerializeField] private KeyCode Left = KeyCode.A;
        [SerializeField] private KeyCode Right = KeyCode.D;
        [SerializeField] private KeyCode Front = KeyCode.W;
        [SerializeField] private KeyCode Back = KeyCode.S;
        [SerializeField] private KeyCode Jump = KeyCode.Space;
        [SerializeField] private KeyCode BreakBlock = KeyCode.Mouse0;

        public override bool IsJumpStart =>
            Input.GetKeyDown(Jump);

        public override Vector3 InputDirection{
            get {
                var result = Vector3.zero;
                if(Input.GetKey(Left))
                    result += Vector3.left;
                if(Input.GetKey(Right))
                    result += Vector3.right;
                if(Input.GetKey(Front))
                    result += Vector3.forward;
                if(Input.GetKey(Back))
                    result += Vector3.back;

                return result.normalized;
            }
        }
    }
}