using System;

namespace Extension.Test {
    public static class ExParse {
        public static object ParseToObject(Type targetType, string value) {
            if (targetType == typeof(string))
                return value;
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value);
            var parse = targetType.GetMethod("Parse", new[] { typeof(string) });
            return parse?.Invoke(null, new[] { value })
                   ?? throw new ArgumentException($"{targetType} Type didn't have 'parse(string)'method");
        } 
    }
}