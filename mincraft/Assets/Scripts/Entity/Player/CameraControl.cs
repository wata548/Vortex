using UnityEngine;
using Entity;

namespace Player {
    public class CameraControl: MonoBehaviour {

        
        //==================================================||Fields 
        
        [SerializeField] private Vector3 _initPos = Vector2.zero;
        private Camera _camera = null;
        private ICameraInputSetting _input;
        public Transform CameraTransform => _camera.transform;

        private float _sensitivity = 1f;
        //==================================================||Methods 
        public void SetSensitivity(float pValue) {
            _sensitivity = pValue;
        }
        
        public void SetUp(ICameraInputSetting pInput) =>
            _input = pInput;
        
        private void CameraUpdate() {

            var mouseDelta = _input.CameraDirection * _sensitivity;
            var rotation = transform.rotation.eulerAngles;
            rotation.y += mouseDelta.x;
            
            var pitch = _camera.transform.rotation.eulerAngles.x + mouseDelta.y;
            _camera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            transform.rotation = Quaternion.Euler(rotation);
        }
        
        //==================================================||Unity 
        private void Awake() {
            _camera = Camera.main!;
            _camera.transform.parent = transform;
            _camera.transform.localPosition = _initPos;
        }

        private void Update() {
            if (Time.timeScale == 0)
                return;
            
            CameraUpdate();
        }
    }
}