using UnityEngine;

namespace MapGenerator.Player {
    public static class InputSetting {
        public static KeyCode Left = KeyCode.A;
        public static KeyCode Right = KeyCode.D;
        public static KeyCode Front = KeyCode.W;
        public static KeyCode Back = KeyCode.S;
        public static KeyCode Jump = KeyCode.Space;
        public static KeyCode BreakBlock = KeyCode.Mouse0;

        public static Vector3 GetDirection() {
            var result = Vector3.zero;
            if(Input.GetKey(Left))
                result += Vector3.left;
            if(Input.GetKey(Right))
                result += Vector3.right;
            if(Input.GetKey(Front))
                result += Vector3.forward;
            if(Input.GetKey(Back))
                result += Vector3.back;

            return result.normalized;
        }
    }
}