using System;
using System.Collections.Generic;

namespace Extension.Test {
    public static class ExType {
        public static IEnumerable<Type> GetSuperTypes(this Type type) {
            var superTypes = new List<Type>();
            var targetType = type;
            while (type != null) {
                superTypes.Add(type);
                type = type.BaseType;
            }

            return superTypes;
        }
    }
}