using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extension.Test {
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestMethodAttribute: Attribute {
        public readonly int Priority;
        public readonly string Name;
        public readonly bool RuntimeOnly;

        public TestMethodAttribute(int priority, string name = "", bool runtimeOnly = false) =>
            (Name, Priority, RuntimeOnly) = (name, priority, runtimeOnly);
        
        public TestMethodAttribute(string name = "", int priority = 0, bool runtimeOnly = false) =>
            (Name, Priority, RuntimeOnly) = (name, priority, runtimeOnly);
        
    }

    public class MethodComparer<T> : IEqualityComparer<(MethodInfo, T)> {
        public bool Equals((MethodInfo, T) x, (MethodInfo, T) y) {
            return x.Item1.Name == y.Item1.Name
                   && x.Item1.ReturnType == y.Item1.ReturnType
                   && x.Item1.GetParameters().Select(p => p.ParameterType)
                       .SequenceEqual(y.Item1.GetParameters().Select(p => p.ParameterType));
        }

        public int GetHashCode((MethodInfo, T) obj) {

            var parameters = obj.Item1.GetParameters();
            
            if(parameters.Length == 0)
                return HashCode.Combine(obj.Item1.Name, obj.Item1.ReturnType);
            
            var hash = obj.Item1.GetParameters()
                .Select(p => p.ParameterType.GetHashCode())
                .Aggregate((lhs, rhs) => (lhs * 123) ^ rhs);
            
            return HashCode.Combine(obj.Item1.Name, obj.Item1.ReturnType, hash);
        }
    }
}