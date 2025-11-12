using System;
using Extension;
using UnityEngine;

namespace Player {
    
    [RequireComponent(typeof(CameraControl))]
    public class Interaction: MonoBehaviour {
        
       //==================================================||Constant 
        private const float INTERACT_RANGE = 5;
        
       //==================================================||Fields 
        [SerializeField] private GameObject _selectBoxPrefab;
        private CameraControl _cameraControl;
        
        private Vector3 _point = Vector3.zero;
        private GameObject _selectBox;
        
       //==================================================||Methods 
        public void Interact() {

            var cameraTransform = _cameraControl.CameraTransfom;
            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, INTERACT_RANGE))
                return;

            _point = (hit.point + cameraTransform.forward * 0.01f).ToVec3Int() + Vector3.one * 0.5f;
        }
    
        //==================================================||Unity 
        private void Awake() {
            _selectBox = Instantiate(_selectBoxPrefab);
        }

        private void Update() {

            if (Input.GetMouseButtonDown(0)) {
                _selectBox.SetActive(true);
            }

            if (Input.GetMouseButton(0)) {
                
                Interact();
                _selectBox.transform.position = _point;
                _selectBox.transform.position = _point;
                _selectBox.transform.rotation = Quaternion.identity;
            }

            if (Input.GetMouseButtonUp(0))
                _selectBox.SetActive(false);
        }
    }
}