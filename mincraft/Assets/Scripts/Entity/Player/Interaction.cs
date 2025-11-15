using Entity;
using Extension;
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
        private Vector3Int _point = Vector3Int.zero;
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
        
        private void Interact() {

            var cameraTransform = _cameraControl.CameraTransform;
            _isShow = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, INTERACT_RANGE, LayerMask.GetMask("Ground"));
            if (!_isShow)
                return;

            _rawPoint = hit.point; 
            _point = (_rawPoint + cameraTransform.forward * 0.01f).ToVec3Int();
            _targetBlock = ChunkManager.Instance.GetMapData(_point);
        }

        private void SelectBoxTransformUpdate() {
            _selectBox.SetActive(_isShow);
            _selectBox.transform.position = _point + Vector3.one * 0.5f;
            _selectBox.transform.rotation = Quaternion.identity;
        }

        private void BreakParticleControl() {
            var idx = TileIdxData.Get(_targetBlock, TileIdxData.FaceType.Side);
            _breakParticleMaterial.SetVector("_Pos", new(idx.X, idx.Y));
            var direction = (_rawPoint - _point - Vector3.one * 0.5f).GetDirection();
            var pos = _point + Vector3.one * 0.5f + direction * 0.5f;
            if (direction.y == 0) {
                _breakVParticle.Stop();
                if(_breakHParticle.isStopped)
                    _breakHParticle.Play();
                                
                _breakHParticle.transform.position = pos;
                _breakHParticle.transform.rotation = Quaternion.Euler(-90, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg , 0);
            }
            else {
                _breakHParticle.Stop();
                if(_breakVParticle.isStopped)
                    _breakVParticle.Play();
                                
                _breakVParticle.transform.position = pos;
            }
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

            var prevPoint = _point;
            
            Interact();
            SelectBoxTransformUpdate();
            
            if (prevPoint != _point) {
                CancelTarget();
                return;
            }
            
            if (_isShow && _input.BreakBlock) {
                BreakParticleControl();    
                
                _interactionTime += Time.deltaTime;
                var targetTime = (float)_targetBlock.GetData(BlockTag.BreakTime);
                var process = _interactionTime / targetTime;
                _breakProcessMaterial.SetFloat("_BreakProcess", process);

                if (process >= 1) {

                    _breakParticle.transform.position = _point + Vector3.one * 0.5f;
                    _breakParticle.Play();
                    ChunkManager.Instance.UpdateBlock((_point + Vector3.one * 0.5f, Block.Air));
                    _isShow = false;
                    
                    CancelTarget();
                }
                    
            }
            else if (_interactionTime > 0 || prevPoint != _point)
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