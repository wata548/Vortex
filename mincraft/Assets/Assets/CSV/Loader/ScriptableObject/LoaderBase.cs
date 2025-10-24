#if UNITY_EDITOR

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace CSVData {
    public abstract class LoaderBase: ScriptableObject {
        
        public abstract void Load(string directory);

        public abstract void Generate();
        
        protected void SyncObject(string targetTypeName, List<List<string>> datas, string directory) {
            var targetTableType = Type.GetType($"{targetTypeName}Table") 
                ?? Type.GetType($"Wata.{targetTypeName}Table")!;
            if(targetTableType.BaseType!.GetGenericTypeDefinition() == typeof(CSVDictionaryTable<>))
                CSV.SyncCSVObjectDictionary(targetTypeName, datas, directory);
            else 
                CSV.SyncCSVObjectList(targetTypeName, datas, directory);

            Debug.Log($"complete {targetTypeName} Syncing");
        }
        protected bool IsExistType(string typeName) {
            
            Type targetType = Type.GetType($"{typeName}, Assembly-CSharp") 
                              ?? Type.GetType($"Wata.{typeName}");

            bool result = targetType != null;
            if(result)
                result = targetType!.GetCustomAttribute(typeof(GeneratedCodeAttribute)) == null;

            return result;
        }
        
    }
}

#endif