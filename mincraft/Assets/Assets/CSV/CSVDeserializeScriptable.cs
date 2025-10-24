#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CSVData.Extensions;
using UnityEditor;
using UnityEngine;

namespace CSVData {
    public static partial class CSV {

        private const string DefaultDirectory = "AutoCSVObjectGenerated";

        public static ScriptableObject GetTable(string targettypeName, string directory = DefaultDirectory) =>
            GetTable(Type.GetType(targettypeName), directory);

        public static ScriptableObject GetTable<T>(string directory = DefaultDirectory)
            where T : ICSVListData =>
            GetTable(typeof(T));
        
        public static ScriptableObject GetTable(Type targetType, string diretory = DefaultDirectory) {

            var tableType = Type.GetType($"{targetType}Table");

            if (targetType == null || tableType == null) {
                throw new TypeAccessException($"'{targetType}' or '{targetType}Table' type isn't exist ");
                //return null;
            }

            diretory = Path.Combine("Assets", diretory);
            if (!Directory.Exists(diretory)) {
                Directory.CreateDirectory(diretory);
            }

            var path = Path.Combine(diretory,$"{targetType}Table.asset");

            ScriptableObject table = null;
            if (File.Exists(path)) {
                
                table = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
            else {
                table = ScriptableObject.CreateInstance(tableType);
                AssetDatabase.CreateAsset(table, path);
                AssetDatabase.Refresh();
            }

            return table;
        }

        #region ScriptableDictionary
        //string input
        public static void SyncCSVObjectDictionary(string targetTypeName,string datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(Type.GetType($"{targetTypeName}, Assembly-CSharp"), Parse(datas), directory);
        
        public static void SyncCSVObjectDictionary<T>(string datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(typeof(T), Parse(datas), directory);
        
        public static void SyncCSVObjectDictionary(Type type, string datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(type, Parse(datas), directory);
        
        //id
        public static void SyncCSVObjectDictionary(string targetTypeName,List<string> datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(Type.GetType($"{targetTypeName}, Assembly-CSharp"), Parse(datas), directory);
        
        public static void SyncCSVObjectDictionary<T>(List<string> datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(typeof(T), Parse(datas), directory);
        
        public static void SyncCSVObjectDictionary(Type type, List<string> datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(type, Parse(datas), directory);
        
        //2d
        public static void SyncCSVObjectDictionary(string targetTypeName, List<List<string>> datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectDictionary(Type.GetType($"{targetTypeName}") ?? Type.GetType($"Wata.{targetTypeName}"), datas, directory);
        
        public static void SyncCSVObjectDictionary<T>(List<List<string>> datas, string directory = DefaultDirectory)
            where T: ICSVDictionaryData =>
            SyncCSVObjectDictionary(typeof(T), datas, directory);
        
        public static void SyncCSVObjectDictionary(Type targetType, List<List<string>> datas,
            string directory = DefaultDirectory)
        {

            if (string.IsNullOrWhiteSpace(directory))
                directory = DefaultDirectory;

            if (targetType.GetInterface(nameof(ICSVDictionaryData)) is null) {

                throw new TypeLoadException($"This type({targetType.Name}) isn't CSVDictionaryObject type");
                //return;
            }
            
            
            var defaultReflectionFlag = 
                BindingFlags.Instance
                | BindingFlags.Public;
            
            var tableObject = GetTable(targetType, directory);
            var tableObjectType = tableObject.GetType();

            if (tableObjectType.BaseType!.GetGenericTypeDefinition() != typeof(CSVDictionaryTable<>)) {
                
                throw new TypeLoadException($"This type({tableObjectType.Name}) isn't CSVDictionaryTable type");
                //return;
            }

            var tableInfo = tableObjectType.GetProperty("Table",
                defaultReflectionFlag       
                | BindingFlags.FlattenHierarchy)!;

            var newDictionary = DeserializeToDictionary(targetType, datas, "SerialNumber", out var keyType);
            var serializableDictionaryType = typeof(SerializableDictionary<,>)
                .MakeGenericType(new Type[] {typeof(int), targetType});
            var newSerializableDictionary = Activator.CreateInstance(serializableDictionaryType, newDictionary);
            
            tableInfo.SetValue(tableObject, newSerializableDictionary);

            EditorUtility.SetDirty(tableObject);
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region ScriptableList

        //string
        public static void SyncCSVObjectList(string typeName,string datas, string directory = DefaultDirectory) =>
            SyncCSVObjectList(Type.GetType($"{typeName}, Assembly-CSharp"), Parse(datas), directory);
        
        public static void SyncCSVObjectList<T>(string datas, string directory = DefaultDirectory)
            where T : ICSVListData =>
            SyncCSVObjectList(typeof(T), Parse(datas), directory);
        
        public static void SyncCSVObjectList(Type type,string datas, string directory = DefaultDirectory) =>
            SyncCSVObjectList(type, Parse(datas), directory);
        
        //1d
        public static void SyncCSVObjectList(string typeName,List<string> datas, string directory = DefaultDirectory) =>
            SyncCSVObjectList(Type.GetType($"{typeName}, Assembly-CSharp"), Parse(datas), directory);
        
        public static void SyncCSVObjectList<T>(List<string> datas, string directory = DefaultDirectory)
            where T : ICSVListData =>
            SyncCSVObjectList(typeof(T), Parse(datas), directory);
        
        public static void SyncCSVObjectList(Type type,List<string> datas, string directory = DefaultDirectory) =>
            SyncCSVObjectList(type, Parse(datas), directory);
        
        //2d
        public static void SyncCSVObjectList<T>(List<List<string>> datas, string directory = DefaultDirectory)
            where T : ICSVListData =>
            SyncCSVObjectList(typeof(T), datas, directory);
        
        public static void SyncCSVObjectList(string targetTypeName, List<List<string>> datas,
            string directory = DefaultDirectory) =>
            SyncCSVObjectList(Type.GetType(targetTypeName) ?? Type.GetType($"Wata.{targetTypeName}"), datas, directory);
        
        public static void SyncCSVObjectList(Type targetType, List<List<string>> datas,
            string directory = DefaultDirectory) {
            if (string.IsNullOrWhiteSpace(directory))
                directory = DefaultDirectory;

            if (targetType.BaseType == typeof(ICSVDictionaryData)) {
                
                Debug.Log($"This type({targetType.Name}) is CSVDictionaryObject type,\nRecommend change CSVObject Type");
            }
            
            var defaultReflectionFlag = 
                BindingFlags.Instance
                | BindingFlags.Public;
            
            var tableObject = GetTable(targetType, directory);
            var tableObjectType = tableObject.GetType();

            if (tableObjectType.BaseType!.GetGenericTypeDefinition() != typeof(CSVListTable<>)){
                throw new TypeLoadException($"This type({targetType.Name}) isn't CSVListTable type");
                //return;
            }
            
            var tableInfo = tableObjectType.GetProperty("Table",
                defaultReflectionFlag       
                | BindingFlags.FlattenHierarchy)!;

            var newDictionary = DeserializeToList(targetType, datas);
            var serializableDictionaryType = typeof(List<>)
                .MakeGenericType(new Type[] {targetType});
            var newSerializableDictionary = Activator.CreateInstance(serializableDictionaryType, newDictionary);
            
            tableInfo.SetValue(tableObject, newSerializableDictionary);

            EditorUtility.SetDirty(tableObject);
            AssetDatabase.SaveAssets();
        }
        #endregion
    }
}

#endif