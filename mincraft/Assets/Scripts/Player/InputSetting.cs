using System;
using UnityEngine;

namespace MapGenerator.Player {

    public interface IInputSetting {
        Vector3 InputDirection { get; }
        bool IsJumpStart { get; }
    }

    [Serializable]
    public abstract class ScriptableInputSetting : ScriptableObject, IInputSetting {
        public abstract Vector3 InputDirection { get; }
        public abstract bool IsJumpStart { get; }
    }
}