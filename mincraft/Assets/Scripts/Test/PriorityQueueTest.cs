using System.Linq;
using System.Text;
using Extension;
using Extension.Test;
using UnityEngine;

namespace Test {
    public static class PriorityQueueTest {
        
        
        [TestMethod]
        public static void Test() {
             var queue = new PriorityQueue<int>();

             for (int i = 0; i < 1000; i++) {
                 queue.Add(Random.Range(0, 2048));
             }

             Debug.Log("added");

             var result = new StringBuilder();
             while (queue.Any()) {
                 result.AppendLine(queue.Dequeue().ToString());
             }

             Debug.Log(result);
        }
    }
}