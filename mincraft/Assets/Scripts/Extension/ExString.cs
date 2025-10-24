using System.Linq;
using System.Text;

namespace Extension {
    public static class ExString {

        public static string RemoveWhiteSpace(this string target) =>
            new(target.Where(c => !char.IsWhiteSpace(c)).ToArray());
        
        public static string VariableNameToString(this string target) {
            var labelContext = target
                .StartsWith("m_") ? target[2..] : target;
            labelContext = labelContext
                .StartsWith("_") ? target[1..] : target
                .Replace("_", " ");
            
            labelContext = char.ToUpper(labelContext[0]) + labelContext[1..];
            var builder = new StringBuilder();
            
            foreach (var c in labelContext) {
                if (char.IsUpper(c))
                    builder.Append(' ');
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}