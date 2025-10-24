using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace CSVData {
    
    [Serializable]
    public class LoadInfo<T> where T: class {

        [FormerlySerializedAs("RawData")] public List<T> RawDatas;
        public string TargetTypeName;
        public CSVDataStyle csvDataStyle;

        /// <summary>
        /// Check It can sync data.
        /// </summary>
        /// <param name="defaultTypeName">If TargetTypeName is null, TargetTypeName Changed to defaultTypeName</param>
        /// <returns></returns>
        public bool SyncAble(string defaultTypeName, Func<T, T> postProcessing = null) {
            if (csvDataStyle == CSVDataStyle.Enum)
                return false;

            return Generatable(defaultTypeName, postProcessing);
        }
        
        /// <summary>
        /// Check It can generated data.
        /// </summary>
        /// <param name="defaultTypeName">If TargetTypeName is null, TargetTypeName Changed to defaultTypeName</param>
        /// <returns></returns>
        public bool Generatable(string defaultTypeName, Func<T, T> postProcessing = null) {

            var temp = RawDatas
                .Select(element => postProcessing?.Invoke(element) ?? element);
                
            if (typeof(T) == typeof(string)) {

                temp = RawDatas
                    .Where(element => !string.IsNullOrWhiteSpace(element as string));
            }
            else {
                temp = temp
                    .Where(element => element != null);
            }
            RawDatas = temp.ToList();

            if(RawDatas.Count < 0)
                return false;

            if (string.IsNullOrWhiteSpace(TargetTypeName))
                TargetTypeName = defaultTypeName;
                
            return true;
        }
    }
}