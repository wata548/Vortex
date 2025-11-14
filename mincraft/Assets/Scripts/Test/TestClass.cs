using Extension.Test;
using MapGenerator;
using UnityEngine;

namespace Test {
    public static class TestClass {
        [TestMethod]
        private static void DoubleFaceTest(Block pTarget = Block.Leaf) {
            Debug.Log(!((bool?)pTarget.GetData(BlockTag.Projected) ?? false));
        }
    }
}