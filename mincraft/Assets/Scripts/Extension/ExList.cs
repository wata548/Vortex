using System;
using System.Collections.Generic;

namespace Extension {
    public static class ExList {
        public static List<T> Shuffle<T>(this List<T> pList) {

            var random = new Random();
            for (int i = pList.Count - 1; i > -1; i--) {
                var idx = random.Next() % (i + 1);
                (pList[i], pList[idx]) = (pList[idx], pList[i]);
            }

            return pList;
        }
    }
}