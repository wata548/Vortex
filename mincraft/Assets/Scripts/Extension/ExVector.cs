using UnityEngine;

namespace Extension {
    public static class ExVector {

        public static Vector3 Multiple(this Vector3 a, Vector3 b) =>
            new(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vector3 Multiple(this Vector3Int a, Vector3 b) =>
                    new(a.x * b.x, a.y * b.y, a.z * b.z);
        
        public static Vector2 ToVec2(this Vector3 target) =>
            new(target.x, target.y);

        public static Vector3Int ToVec3Int(this Vector3 pTarget) =>
            new(Mathf.FloorToInt(pTarget.x), Mathf.FloorToInt(pTarget.y), Mathf.FloorToInt(pTarget.z));
        
        public static Vector3 GetDirection(this Vector3 pTarget) {
            var absX = Mathf.Abs(pTarget.x);
            var absY = Mathf.Abs(pTarget.y);
            var absZ = Mathf.Abs(pTarget.z);
            if (absX > absY && absX > absZ) {
                return new(Mathf.Sign(pTarget.x), 0, 0);
            }
            if(absY > absZ) {
                return new(0, Mathf.Sign(pTarget.y), 0);
            }
            return new(0, 0, Mathf.Sign(pTarget.z));
        }
    }
}