using System;

namespace Extension {
    public static class ExEnum {
        public static bool IsFlag<T>(this T a) where T: Enum {
            var value = Convert.ToInt32(a);
            if (value == 0) return false;
            return ((value - 1) & value) == 0;
        }

        public static bool HasFlag<T>(this T a, T pTarget) where T : Enum {
            var value = Convert.ToInt32(a);
            var targetFlag = Convert.ToInt32(pTarget);
            return (value & targetFlag) == targetFlag;
        }
    }
}