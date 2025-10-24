using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVData.Extensions {
    
    public static class CSharpExtension {

        public static string RemoveWhiteSpace(this string target) => 
            new(target.Where(c => !char.IsWhiteSpace(c)).ToArray());

        public static string ReplaceWhiteSpace(this string target, char replacement) =>
            new(target.Select(c => char.IsWhiteSpace(c) ? replacement : c).ToArray());

        //change target text to typeNameFormat
        public static string FixTypeName(this string target) {
            target = target.ReplaceWhiteSpace('_');
            if (target[0] is >= '0' and <= '9')
                target = '_' + target;

            return target;
        }
    
        public static Object Parse(Type type, string rawValue) {

            if (type ==  typeof(string))
                return rawValue;

            if (type.IsEnum)
                return Enum.Parse(type, rawValue);

            if (type.IsGenericType) {

                var genericType = type.GetGenericTypeDefinition();
                if (genericType != typeof(List<>))
                    throw new Exception($"This function didn't support '{type}' Type");

                var genericArgumentType = type.GetGenericArguments()[0];
                var listType = genericType.MakeGenericType(genericArgumentType);
                var result = Activator.CreateInstance(listType)!;
            
                var addFunction = listType.GetMethod("Add",
                    BindingFlags.Instance 
                    | BindingFlags.Public)!;
            
                var values = rawValue.Split('\n');
                foreach (var value in values) {
                    var parsedValue = Parse(genericArgumentType, value);
                    addFunction.Invoke(result, new[] { parsedValue });
                }

                return result;
            }

            var parseFunction = 
                type.GetMethod("Parse", new Type[] { typeof(string) });

            if (parseFunction != null)
                return parseFunction!.Invoke(null, new[] { rawValue });
        
            throw new Exception($"This function didn't support '{type}' Type");
            //return null;
        }
    }
}