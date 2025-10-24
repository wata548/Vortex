using System.Drawing;

namespace CSVData.Extensions {
    public static class TextExtension {

        public static string SetFontSizeBySize(this string context, float size) =>
            $"<size={size}>{context}</size>";

        public static string SetFontSizeByPercent(this string context, int percent) =>
            $"<size={percent}%>{context}</size>";

        public static string SetFontSizeByPercent(this string context, float percent) =>
            $"<size={percent*100}%>{context}</size>";

        public static string SetColor(this string context, Color color) =>
            $"<color=#{color.R:X02}{color.G:X02}{color.B:X02}{color.A:X02}>{context}</color>";
        
        public static string SetColor(this string context, UnityEngine.Color color) =>
            $"<color=#{(int)(color.r * 255):X02}{(int)(color.g * 255):X02}{(int)(color.b * 255):X02}{(int)(color.a * 255):X02}>{context}</color>";
    }
}