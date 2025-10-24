using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CSVData {
    
    [Serializable]
    public enum CSVDataStyle {
        List,
        Dictionary,
        Enum
    }
    
    //use string builder not codedom
    public static partial class CSV {

        
        public static void GenerateObjCode(string targetTypeName, List<List<string>> datas, CSVDataStyle dataStyle, bool makeTable = true, bool makeType = true) {

            string result = DefaultHeader();
            if (dataStyle == CSVDataStyle.Enum) {
                
                GenerateEnum(targetTypeName, datas);
                Debug.Log($"complete {targetTypeName}(enum) Type making");
                return;
            }
            
            if (makeType) { 
                GenerateObjDataType(targetTypeName, datas, dataStyle);
                Debug.Log($"complete {targetTypeName}Type making");
            }
            else
                Debug.Log($"{targetTypeName}Table Type is already exist");
            if (makeTable) {
                
                GenerateObjDataTable(targetTypeName, dataStyle);
                Debug.Log($"complete {targetTypeName}Table  Type making");
            }
            else
                Debug.Log($"{targetTypeName} Type is already exist");
        }

       private static void GenerateObjDataTable(string targetTypeName, CSVDataStyle dataStyle) {
           
            StringBuilder codeGenerator = new();
            codeGenerator.AppendLine("[GeneratedCode]");
            string inherit = dataStyle == CSVDataStyle.List ? "CSVListTable" : $"CSVDictionaryTable";
            codeGenerator.AppendLine($"public class {targetTypeName}Table: {inherit}<{targetTypeName}> {{}}");
            codeGenerator.AppendLine("");
            var result = DefaultHeader() + codeGenerator.ToString();

            GenerateFile($"{targetTypeName}Table", result);

        }
        
        private static void GenerateObjDataType(string targetTypeName, List<List<string>> datas, CSVDataStyle dataStyle) {

            StringBuilder codeGenerator = new();
            codeGenerator.AppendLine("[GeneratedCode]");
            codeGenerator.AppendLine("[Serializable]");
            
            string inherit = dataStyle == CSVDataStyle.List ? nameof(ICSVListData) : nameof(ICSVDictionaryData);
            codeGenerator.AppendLine($"public class {targetTypeName}: {inherit} {{");
            for (int i = 0; i < datas[0].Count; i++) {
             
                if(string.IsNullOrWhiteSpace(datas[0][i]))
                    continue;
                
                //if(datas[0][i] == "SerialNumber" && dataStyle == CSVDataStyle.Dictionary)
                //    continue;
                
                codeGenerator.AppendLine("[field: SerializeField]");
                codeGenerator.AppendLine($"public {datas[1][i]} {datas[0][i]} {{ get; private set; }}");
                
            }
            codeGenerator.AppendLine("};");

            var result = DefaultHeader() + codeGenerator.ToString();

            GenerateFile(targetTypeName, result);
        }
    }
}