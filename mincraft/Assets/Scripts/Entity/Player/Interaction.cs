using System;
using Extension;
using MapGenerator;
using UnityEngine;

namespace Player {
    
    [RequireComponent(typeof(CameraControl))]
    public class Interaction: MonoBehaviour {
        
        //==================================================||Constant 
        private const float INTERACT_RANGE = 5;
        
        //==================================================||Fields 
        [SerializeField] private GameObject _selectBoxPrefab;
        [SerializeField] private Material _breakEffectMaterial;
        private CameraControl _cameraControl;
        
        private Vector3Int _point = Vector3Int.zero;
        private Block _targetBlock;
        private bool _isShow = false; 
        private GameObject _selectBox;
        private float _interactionTime;
        
        //==================================================||Methods 
        public void Interact() {

            var cameraTransform = _cameraControl.CameraTransfom;
            _isShow = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, INTERACT_RANGE, LayerMask.GetMask("Ground"));
            if (!_isShow)
                return;

            _point = (hit.point + cameraTransform.forward * 0.01f).ToVec3Int();
            _targetBlock = ChunkManager.Instance.GetMapData(_point);
        }

        private void SelectBoxTransformUpdate() {
            _selectBox.SetActive(_isShow);
            _selectBox.transform.position = _point + Vector3.one * 0.5f;
            _selectBox.transform.rotation = Quaternion.identity;
        }
    
        //==================================================||Unity 
        private void Awake() {
            _cameraControl = GetComponent<CameraControl>();
            _selectBox = Instantiate(_selectBoxPrefab);
        }

        private void Update() {

            var prevPoint = _point;
            
            Interact();
            SelectBoxTransformUpdate();
            
            if (prevPoint != _point) {
                CancelTarget();
                return;
            }
            
            if (_isShow && Input.GetMouseButton(0)) {
                _interactionTime += Time.deltaTime;
                var targetTime = (float)BlockData.GetData(_targetBlock, BlockTag.BreakTime);
                var process = _interactionTime / targetTime;
                _breakEffectMaterial.SetFloat("_BreakProcess", process);

                if (process >= 1) {
                    ChunkManager.Instance.UpdateBlock((_point + Vector3.one * 0.5f, Block.Air));
                    _isShow = false;
                    CancelTarget();
                }
                    
            }
            else if (_interactionTime > 0 || prevPoint != _point)
                CancelTarget();


            void CancelTarget() {
                _interactionTime = 0;
                _breakEffectMaterial.SetFloat("_BreakProcess", 0);
            }
        }
    }
}