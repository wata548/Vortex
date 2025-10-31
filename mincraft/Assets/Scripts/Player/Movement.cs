using Extension;
using UnityEngine;

namespace MapGenerator.Player {
    [RequireComponent(typeof(Rigidbody))]
    public class Movement: MonoBehaviour {

        [SerializeField] private ScriptableInputSetting _inputSetting;   
        
        //peekTime = 0.3, maxHeight = 1.3 => gravityScale = -28.8888..., jumpScale = 8.666...
        // j = 2h / t, g = -j / t
        
        private const float JUMP_SCALE = 8.66f;
        private const float CONTACT_RANGE = 0.05f;
        private const float SPEED = 6f;

        private bool _isGround;
        private Camera _camera;
        private Rigidbody _rigidbody;
#if UNITY_EDITOR
        private Vector3 E_inputDirection = Vector3.forward;
        private bool E_isStop = false;
#endif
        
       //==================================================||Medhods 
        private void GroundCheck() {
            var pos = transform.position;
            pos.y -= transform.localScale.y * 0.5f;
            var scale = transform.localScale * 0.49f;
            scale.y = CONTACT_RANGE;
            var groundCount = Physics.OverlapBox(pos, scale, transform.rotation, LayerMask.GetMask("Ground")).Length;
            _isGround = groundCount != 0;
        }
        
        private Vector3 InputPostProcessing() {
            var rotation = transform.rotation;
            var direction = _inputSetting.InputDirection;
            
            var delta = SPEED * (rotation * direction);
         
            //wall push prevent
            var pos = transform.position + 0.5f * (rotation * direction.Multiple(transform.localScale));
            var scale = transform.localScale * 0.49f;
            scale.z = CONTACT_RANGE;
            
            var directionRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0);
            if (Physics.OverlapBox(pos, scale, directionRotation * rotation, LayerMask.GetMask("Ground")).Length > 0) {
                delta = Vector3.zero;
            }
#if UNITY_EDITOR
            E_inputDirection = direction;
            E_isStop = delta == Vector3.zero;
#endif

            return delta;
        }
        
        private void CameraUpdate() {

            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
            var rotation = transform.rotation.eulerAngles;
            rotation.y += mouseDelta.x;
            
            var pitch = _camera.transform.rotation.eulerAngles.x + mouseDelta.y;
            _camera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            transform.rotation = Quaternion.Euler(rotation);
        }

       //==================================================||Unity 
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main!;
        }

        private void Update() {
            
            CameraUpdate();
            var delta = InputPostProcessing();
            var velocity = _rigidbody.velocity;
            (velocity.x, velocity.z) = (delta.x, delta.z);
            
            if (_isGround && _inputSetting.IsJumpStart)
                velocity.y += JUMP_SCALE;

            _rigidbody.velocity = velocity;
            GroundCheck();
        }
        
        private void OnDrawGizmos() {
            
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            var pos = Vector3.down * 0.5f;
            var scale = Vector3.one * 0.9f;
            scale.y = transform.InverseTransformVector(Vector3.up * CONTACT_RANGE * 2).magnitude;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos, scale);
            
#if UNITY_EDITOR
            var rotation = transform.rotation;
            pos = transform.position + 0.5f * ( rotation * E_inputDirection.Multiple(transform.localScale));
            scale = transform.localScale * 0.49f;
            scale.z = CONTACT_RANGE;
            
            var directionRotation = Quaternion.Euler(0,
                Mathf.Atan2(E_inputDirection.x, E_inputDirection.z) * Mathf.Rad2Deg,
                0);
            
            Gizmos.color = E_isStop ? Color.blue : Color.green;
            Gizmos.matrix = Matrix4x4.TRS(pos, directionRotation * transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, scale * 2f);
#endif
        }
    }
}