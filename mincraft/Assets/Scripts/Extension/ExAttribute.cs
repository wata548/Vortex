using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension {
    public static class ExAttribute {
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this Type type, BindingFlags flag)
                    where T: Attribute => type 
                    ?.GetMethods(flag)
                    .Where(method => method.IsDefined(typeof(T)))
                    .Select(method => (
                        method,
                        method.GetCustomAttribute(typeof(T)) as T
                    ));
        
        public static IEnumerable<(MethodInfo Method, T Attribute)> HaveAttributeMethods<T>(this object target, BindingFlags flag)
            where T: Attribute => target
            .GetType()
            .GetMethods(flag)
            .Where(method => method.IsDefined(typeof(T)))
            .Select(method => (
                method,
                method.GetCustomAttribute(typeof(T)) as T
            ));
    }
}