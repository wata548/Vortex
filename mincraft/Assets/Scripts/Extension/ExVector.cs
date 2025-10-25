using UnityEngine;

namespace Extension {
    public static class ExVector {

        public static Vector3 Multiple(this Vector3 a, Vector3 b) =>
            new(a.x * b.x, a.y * b.y, a.z * b.z);
        
        public static Vector2 ToVec2(this Vector3 target) =>
            new(target.x, target.y);
    }
}