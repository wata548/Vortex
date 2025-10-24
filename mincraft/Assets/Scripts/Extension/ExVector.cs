using UnityEngine;

namespace Extension {
    public static class ExVector {

        public static Vector2 ToVec2(this Vector3 target) =>
            new(target.x, target.y);
    }
}