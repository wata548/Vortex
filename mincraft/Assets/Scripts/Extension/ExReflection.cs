using System;
using System.Reflection;

namespace Extension {
    public static class ExReflection {
        public static Type GetBaseClass(this PropertyInfo property) =>
            (property.GetSetMethod() ?? property.GetGetMethod())!
            .GetBaseDefinition().DeclaringType;
    }
}