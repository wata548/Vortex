using Entity;
using Extension;
using Inventory;
using MapGenerator;
using MapGenerator.Tile;
using UnityEngine;

namespace Player {
    
    [RequireComponent(typeof(CameraControl))]
    public class Interaction: MonoBehaviour {
        
        //==================================================||Constant 
        private const float INTERACT_RANGE = 5;
        
        //==================================================||Fields 
        [Header("BreakBox")]
        [SerializeField] private GameObject _selectBoxPrefab;
        [SerializeField] private Material _breakProcessMaterial;
        [Header("BreakParticle")]
        [SerializeField] private Material _breakParticleMaterial;
        [SerializeField] private ParticleSystem _breakHParticlePrefab;
        [SerializeField] private ParticleSystem _breakVParticlePrefab;
        [SerializeField] private ParticleSystem _breakParticlePrefab;
        
        private CameraControl _cameraControl;
        
        private Vector3 _rawPoint = Vector3Int.zero;
        private Vector3Int _pointPos = Vector3Int.zero;
        private Vector3 _point => _pointPos + Vector3.one * 0.5f;
        
        private Block _targetBlock;
        private bool _isShow = false; 
        private float _interactionTime;
        
        private GameObject _selectBox;
        private ParticleSystem _breakHParticle;
        private ParticleSystem _breakVParticle;
        private ParticleSystem _breakParticle;

        private IPlayerInputSetting _input;
        //==================================================||Methods 
        public void SetUp(IPlayerInputSetting pInput) {
            _input = pInput;
        }
        
        private void FindPoint() {

            var cameraTransform = _cameraControl.CameraTransform;
            _isShow = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, INTERACT_RANGE, LayerMask.GetMask("Ground"));
            if (!_isShow)
                return;

            _rawPoint = hit.point; 
            _pointPos = (_rawPoint + cameraTransform.forward * 0.01f).ToVec3Int();
            _targetBlock = ChunkManager.Instance.GetMapData(_pointPos);
        }

        private void SelectBoxTransformUpdate() {
            _selectBox.SetActive(_isShow);
            _selectBox.transform.position = _point;
            _selectBox.transform.rotation = Quaternion.identity;
        }

        private void BreakParticleControl() {
            var idx = _targetBlock.GetFace(TileIdxData.FaceType.Side);
            _breakParticleMaterial.SetVector("_Pos", new(idx.X, idx.Y));
            var bigAxis = (_rawPoint - _point).GetBigAxis();
            var pos = _point + bigAxis * 0.5f;
            
            //horizontal
            if (bigAxis.y == 0) {
                _breakVParticle.Stop();
                if(_breakHParticle.isStopped)
                    _breakHParticle.Play();
                                
                _breakHParticle.transform.position = pos;
                _breakHParticle.transform.rotation = Quaternion.Euler(-90, Mathf.Atan2(bigAxis.x, bigAxis.z) * Mathf.Rad2Deg , 0);
                return;
            }
            
            //vertical
            _breakHParticle.Stop();
            if(_breakVParticle.isStopped)
                _breakVParticle.Play();
                                
            _breakVParticle.transform.position = pos;
        }

        private float ShowBreakProcess() {
            _interactionTime += Time.deltaTime;
            var targetTime = (float)_targetBlock.GetData(BlockTag.BreakTime);
            var process = _interactionTime / targetTime;
            _breakProcessMaterial.SetFloat("_BreakProcess", process);
            
            return process;
        }

        private void OnBreakBlock() {
            _breakParticle.transform.position = _point;
            _breakParticle.Play();
            
            ChunkManager.Instance.SetBlocks((_point, Block.Air));
            
            _isShow = false;
            InventoryData.GetItem(_targetBlock);
        }

        private void PlaceBlock() {
            if (!InventoryData.UseItem(out var block))
                return;

            var bigAxis = (_rawPoint - _point).GetBigAxis();
            ChunkManager.Instance.SetBlocks((bigAxis + _point, block));
        }
        
        //==================================================||Unity 
        private void Awake() {
            _cameraControl = GetComponent<CameraControl>();
            _selectBox = Instantiate(_selectBoxPrefab);

            var folder = new GameObject("Effects").transform;
            _breakHParticle = Instantiate(_breakHParticlePrefab, folder);
            _breakVParticle = Instantiate(_breakVParticlePrefab, folder);
            _breakParticle = Instantiate(_breakParticlePrefab, folder);
        }

        private void Update() {

            var prevPoint = _pointPos;
            
            FindPoint();
            SelectBoxTransformUpdate();
            
            if (prevPoint != _pointPos) {
                CancelTarget();
                return;
            }

            if (_isShow && _input.PlaceBlock)
                PlaceBlock();
            
            //BreakBlock
            else if (_isShow && _input.BreakBlock) {
                BreakParticleControl();

                if (ShowBreakProcess() >= 1) {
                    OnBreakBlock();
                    CancelTarget();   
                }
                    
            }
            else if (_interactionTime > 0 || prevPoint != _pointPos)
                CancelTarget();
            void CancelTarget() {
                
                _breakHParticle.Stop();
                _breakVParticle.Stop();
                _interactionTime = 0;
                _breakProcessMaterial.SetFloat("_BreakProcess", 0);
            }
        }
    }
}