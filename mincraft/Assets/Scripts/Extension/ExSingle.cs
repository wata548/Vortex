using UnityEngine.Timeline;

namespace Extension {
    public static class ExSingle {
        public static float Sign(this float pValue) =>
            pValue == 0 ? 0f : pValue > 0 ? 1f : -1f;
    }
}