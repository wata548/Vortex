using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension.Test {
    public static class ExAttribute {
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this Type type, BindingFlags flag)
            where T: Attribute => type?.GetMethods(flag)
                .Where(method => method.IsDefined(typeof(T)))
                .Select(method => (
                    method,
                    method.GetCustomAttribute(typeof(T)) as T
                ));
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this Type type, BindingFlags flag, IEqualityComparer<(MethodInfo, T)> comparer)
            where T: Attribute => type
            .GetSuperTypes()
            .SelectMany(type => type.HaveAttributeMethods<T>(flag))
            .Distinct(comparer);
    }
}