using System;

namespace Extension.Test {
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestMethod: Attribute {
        public readonly int Priority;
        public readonly string Name;
        public readonly bool RuntimeOnly;

        public TestMethod(int priority, string name = "", bool runtimeOnly = false) =>
            (Name, Priority, RuntimeOnly) = (name, priority, runtimeOnly);
        
        public TestMethod(string name = "", int priority = 0, bool runtimeOnly = false) =>
            (Name, Priority, RuntimeOnly) = (name, priority, runtimeOnly);
        
    }
}