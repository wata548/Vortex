using System;
using UnityEngine;

namespace Player {
    public class CameraControl: MonoBehaviour {

       //==================================================||Fields 
        [SerializeField] private Vector2 _initPos = Vector2.zero;
        private Camera _camera = null;
        
       //==================================================||Methods 
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
            _camera = Camera.main!;
            _camera.transform.parent = transform;
            _camera.transform.localPosition = _initPos;
        }

        private void Update() {
            CameraUpdate();
        }
    }
}