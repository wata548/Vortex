using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CSVData.Extensions;
using Object = System.Object;

namespace CSVData {
    public static partial class CSV {

        #region Object

        private static Object Deserialize(string typeName, List<string> headerNames, List<string> values,
            bool ignoreSomeFieldError = true) =>
            Deserialize(Type.GetType(typeName), headerNames, values, ignoreSomeFieldError);
        
        private static Object Deserialize<T>(List<string> headerNames, List<string> values,
            bool ignoreSomeFieldError = true) =>
            Deserialize(typeof(T), headerNames, values, ignoreSomeFieldError);
        
        private static Object Deserialize(Type targetType, List<string> headerNames, List<string> values,
            bool ignoreSomeFieldError = true) {

            var defaultReflectionFlag = BindingFlags.Instance 
                                        | BindingFlags.NonPublic
                                        | BindingFlags.Public
                                        | BindingFlags.FlattenHierarchy;
            
            bool isAllowedInput = 
                headerNames.Count == values.Count;

            if (!isAllowedInput)
                throw new ArgumentException($"This parameter isn't allowed, please match parameter's size equal.\n"
                                            + $"Names: {headerNames.Count}, Values: {values.Count}");

            var result = Activator.CreateInstance(targetType);

            for (int i = 0; i < headerNames.Count; i++) {

                var fieldInfo = 
                    targetType.GetField(headerNames[i], defaultReflectionFlag);
                
                if (fieldInfo == null) {

                    var propertyInfo = 
                        targetType.GetProperty(headerNames[i], defaultReflectionFlag);

                    if (propertyInfo == null || !propertyInfo.CanWrite) {
                        Debug.LogError($"{headerNames[i]} isn't exist or can't write. ({propertyInfo})");
                        if (!ignoreSomeFieldError)
                            throw new MissingFieldException($"This '{headerNames[i]}' isn't exist on {result}Type");
                        continue;        
                    }

                    var propertyType = propertyInfo.PropertyType;
                    var propertyData = CSharpExtension.Parse(propertyType, values[i]);
                    
                    propertyInfo.SetValue(result, propertyData);
                }

                else {
                    
                    var fieldType = fieldInfo.FieldType;
                    var fieldData = CSharpExtension.Parse(fieldType, values[i]);
                    fieldInfo.SetValue(result, fieldData);
                }
            }

            (result as ICSVListData)!.PostProcessing();

            return result;
        }

        #endregion

        #region List

        public static Object DeserializeToList(string typeName, string data, bool ignoreSomeError = true) =>
            DeserializeToList(Type.GetType(typeName), Parse(data), ignoreSomeError);

        public static Object DeserializeToList<T>(string data, bool ignoreSomeError = true) =>
            DeserializeToList(typeof(T), Parse(data), ignoreSomeError);

        public static Object DeserializeToList(Type type, string data, bool ignoreSomeError = true) =>
            DeserializeToList(type, Parse(data), ignoreSomeError);
        
        public static Object DeserializeToList(string typeName, List<string> data, bool ignoreSomeError = true) =>
            DeserializeToList(Type.GetType(typeName), Parse(data), ignoreSomeError);

        public static Object DeserializeToList<T>(List<string> data, bool ignoreSomeError = true) =>
            DeserializeToList(typeof(T), Parse(data), ignoreSomeError);

        public static Object DeserializeToList(Type type, List<string> data, bool ignoreSomeError = true) =>
            DeserializeToList(type, Parse(data), ignoreSomeError);
        
        public static Object DeserializeToList(string typeName, List<List<string>> datas, bool ignoreSomeError = true) =>
            DeserializeToList(Type.GetType(typeName), datas, ignoreSomeError);

        public static Object DeserializeToList<T>(List<List<string>> datas, bool ignoreSomeError = true) =>
            DeserializeToList(typeof(T), datas, ignoreSomeError);
        
        public static Object DeserializeToList(Type targetType, List<List<string>> datas, bool ignoreSomeError = true) {

            if (datas.Count <= 2) {

                if (!ignoreSomeError)
                    throw new Exception("data should big than 2 row(headerName, headerType)");
                
                return null;
            }
            
            var headerNames = datas[0];

            var listType = typeof(List<>).MakeGenericType(targetType);
            
            var list = Activator.CreateInstance(listType);
            var addFunction = listType.GetMethod("Add", new Type[] { targetType })!;
            
            for (int i = 2; i < datas.Count; i++) {
                var row = Deserialize(targetType, headerNames, datas[i], ignoreSomeError);
                addFunction.Invoke(list, new[] {row});
            }

            return list;
        }
        
        public static Object DeserializeToListBySpreadSheet(Type targetType, string path, string sheet, string key,
            bool ignoreSomeError = true) {
            var data = SpreadSheet.LoadData(path, sheet, key);
            return DeserializeToList(targetType, data, ignoreSomeError);
        }

        #endregion

        #region Dictionary
        
        //string input
        public static Object DeserializeToDictionary(string typeName, string data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary<T>(string data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(typeof(T), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary(Type type, string data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        //string input and ignore out keyType
        public static Object DeserializeToDictionary(string typeName, string data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary<T>(string data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary(typeof(T), Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary(Type type, string data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        //1dimension input
        public static Object DeserializeToDictionary(string typeName, List<string> data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Dictionary<Object, T> DeserializeToDictionary<T>(List<string> data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            (Dictionary<Object, T>)DeserializeToDictionary(typeof(T), Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary(Type type, List<string> data, string primaryKey, 
            out Type keyType, bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out keyType, ignoreSomeError);
        
        //1dimension and ignore out keyType
        public static Object DeserializeToDictionary(string typeName, List<string> data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary(Type.GetType(typeName), Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        public static Dictionary<Object, T> DeserializeToDictionary<T>(List<string> data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            (Dictionary<Object, T>)DeserializeToDictionary(typeof(T), Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        public static Object DeserializeToDictionary(Type type, List<string> data, string primaryKey, 
            bool ignoreSomeError  = true) =>
            DeserializeToDictionary(type, Parse(data), primaryKey, out var keyType, ignoreSomeError);
        
        //2dimension list Type ignore key Type
        
        public static Object DeserializeToDictionary(string typeName, List<List<string>> datas, string primaryKeyName,
            bool ignoreSomeError = true) =>
            DeserializeToDictionary(Type.GetType(typeName), datas, primaryKeyName, out var keyType, ignoreSomeError);

        public static Object DeserializeToDictionary<T>(List<List<string>> datas, string primaryKeyName,
            bool ignoreSomeError = true) =>
            DeserializeToDictionary(typeof(T), datas, primaryKeyName, out var keyType, ignoreSomeError);
        public static Object DeserializeToDictionary(Type type, List<List<string>> datas, string primaryKeyName,
            bool ignoreSomeError = true) =>
            DeserializeToDictionary(type, datas, primaryKeyName, out var keyType, ignoreSomeError);
        
        //2dimension list Type
        public static Object DeserializeToDictionary(string typeName, List<List<string>> datas, string primaryKeyName,
            out Type keyType, bool ignoreSomeError = true) =>
            DeserializeToDictionary(Type.GetType(typeName), datas, primaryKeyName, out keyType, ignoreSomeError);

        public static Dictionary<Object, T> DeserializeToDictionary<T>(List<List<string>> datas, string primaryKeyName,
            out Type keyType, bool ignoreSomeError = true) {

            var output = DeserializeToDictionary(typeof(T), datas, primaryKeyName, out keyType, ignoreSomeError);
            return (Dictionary<Object, T>)output;

        }
        
        public static Object DeserializeToDictionary(Type targetType, List<List<string>> datas, string primaryKeyName,
            out Type keyType, bool ignoreSomeError = true) {
            if (datas.Count <= 2) {

                if (!ignoreSomeError)
                    throw new Exception("data should big than 2 row(headerName, headerType)");

                keyType = null;
                return null;
            }

            MemberInfo primaryKeyInfo = targetType.GetField(primaryKeyName);
            if (primaryKeyInfo == null) {

                primaryKeyInfo = targetType.GetProperty(primaryKeyName);
                if (primaryKeyInfo == null) {
                    if (!ignoreSomeError)
                        throw new Exception($"This key is wrong '{primaryKeyName}'isn't exist");
                    keyType = null;
                    return null;
                }

                keyType = (primaryKeyInfo as PropertyInfo)!.PropertyType;
            }
            else
                keyType = (primaryKeyInfo as FieldInfo)!.FieldType;
            
            var headerNames = datas[0];
            var dictionaryType = 
                typeof(Dictionary<,>).MakeGenericType(keyType, targetType);
            var result = Activator.CreateInstance(dictionaryType);

            Dictionary<int, int> a = new();
            var addFunction = dictionaryType
                .GetMethod("Add", new[] { keyType, targetType })!;

            for (int i = 2; i < datas.Count; i++) {
                var newValue = Deserialize(targetType, headerNames, datas[i], ignoreSomeError);

                Object newKey = null;

                if (primaryKeyInfo is FieldInfo fieldKey)
                    newKey = fieldKey.GetValue(newValue)!;

                else if (primaryKeyInfo is PropertyInfo propertyKey)
                    newKey = propertyKey.GetValue(newValue)!;
                
                
                addFunction.Invoke(result, new[] { newKey, newValue });
            }

            return result;
        }

        public static Object DeserializeToDictionaryBySpreadSheet(Type targetType, string path, string sheet, string key,
            out Type keyType, string primaryKey = "SerialNumber", bool ignoreSomeError = true) {
            var data = SpreadSheet.LoadData(path, sheet, key);
            return DeserializeToDictionary(targetType, data, primaryKey, out keyType, ignoreSomeError);
        }
        #endregion
    }
}