using Entity;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(CameraControl))]
    [RequireComponent(typeof(Interaction))]
    public class PlayerMovement : Movement {

        [SerializeField] private ScriptableInputSetting _inputSettingObject;
        protected override IInputSetting _inputSetting => _inputSettingObject;

        public void SetKeyMap(ScriptableInputSetting pInput) {
            _inputSettingObject = pInput;
            GetComponent<CameraControl>().SetUp(_inputSettingObject);
            GetComponent<Interaction>().SetUp(_inputSettingObject);
        }

        public void SetCameraSensitivity(float pValue) {
            GetComponent<CameraControl>().SetSensitivity(pValue);
        }
        
        private new void Awake() {
            base.Awake();
            SetKeyMap(_inputSettingObject);
        }

        private new void Update() {
            base.Update();
            if (_inputSettingObject.Menu) {
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            }
        }
    }
}