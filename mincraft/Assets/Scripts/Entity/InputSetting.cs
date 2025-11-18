using System;
using UnityEngine;

namespace Entity {

    public interface IInputSetting {
        Vector3 InputDirection { get; }
        bool IsJumpStart { get; }
    }

    public interface ICameraInputSetting {
        Vector2 CameraDirection { get; }
    }

    public interface IPlayerInputSetting {
        bool BreakBlock { get; }
        bool PlaceBlock { get; }
        int SelectItemSlot { get; }
        bool Menu { get; }
    }
}