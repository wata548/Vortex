using System;
using UnityEngine;

namespace Entity {
    [Serializable]
    public abstract class ScriptableInputSetting : ScriptableObject, IInputSetting, ICameraInputSetting, IPlayerInputSetting {
        public abstract Vector3 InputDirection { get; }
        public abstract bool IsJumpStart { get; }
        public abstract Vector2 CameraDirection { get; }
        public abstract bool BreakBlock { get; }

        public abstract bool Menu { get; }
    }
}