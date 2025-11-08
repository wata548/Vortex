using Entity;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(CameraControl))]
    public class PlayerMovement : Movement {

        [SerializeField] private ScriptableInputSetting _inputSettingObject;
        protected override IInputSetting _inputSetting => _inputSettingObject;
    }
}