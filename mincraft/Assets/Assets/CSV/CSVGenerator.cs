using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CSVData {
    
    //use string builder not codedom
    public static partial class CSV {

        private const string DefaultPath = "Assets/Scripts/AutoCSVOutputScript"; 
        public static void GenerateCode(string targetTypeName, List<List<string>> datas, CSVDataStyle dataStyle = CSVDataStyle.List) {

            if (dataStyle == CSVDataStyle.Enum) {
                
                GenerateEnum(targetTypeName, datas);
                Debug.Log($"complete {targetTypeName}(enum) Type making");
                return;
            }

            GenerateDataType(targetTypeName, datas, dataStyle);
            Debug.Log($"complete {targetTypeName}Type making");
        }

        private static void GenerateFile(string targetTypeName, string context) {
            if (!Directory.Exists(DefaultPath))
                Directory.CreateDirectory(DefaultPath);
            
            var path = Path.Combine(DefaultPath, $"{targetTypeName}.cs");
            if (File.Exists(path))
                File.Delete(path);
            
            File.WriteAllText(path, context);
        }
        
        private static string DefaultHeader() {
            
            StringBuilder codeGenerator = new();
            codeGenerator.AppendLine("using System;");
            codeGenerator.AppendLine("using UnityEngine;");
            codeGenerator.AppendLine("using System.Collections.Generic;");
            codeGenerator.AppendLine("using CSVData;");
            codeGenerator.AppendLine("");
            codeGenerator.AppendLine("");
            return codeGenerator.ToString();
        }

        private static void GenerateDataType(string targetTypeName, List<List<string>> datas, CSVDataStyle dataStyle) {

            StringBuilder codeGenerator = new();
            codeGenerator.AppendLine("\t[GeneratedCode]");
            codeGenerator.AppendLine("\t[Serializable]");
            
            codeGenerator.AppendLine($"\tpublic class {targetTypeName} {{");
            for (int i = 0; i < datas[0].Count; i++) {
             
                if(string.IsNullOrWhiteSpace(datas[0][i]))
                    continue;
                
                //if(datas[0][i] == "SerialNumber" && dataStyle == CSVDataStyle.Dictionary)
                //continue;
                
                codeGenerator.AppendLine("\t[field: SerializeField]");
                codeGenerator.AppendLine($"\tpublic {datas[1][i]} {datas[0][i]} {{ get; private set; }}");
                
            }
            codeGenerator.AppendLine("\t};");

            var result = DefaultHeader() + codeGenerator.ToString();

            GenerateFile(targetTypeName, result);
        }
        
        private static void GenerateEnum(string targetTypeName, List<List<string>> datas) {
            StringBuilder codeGenerator = new();

            codeGenerator.AppendLine($"enum {targetTypeName} {{");
            foreach (var data in datas) {
                
                codeGenerator.AppendLine($"\t{data[0]},");
            }
            codeGenerator.AppendLine("};");
            
            var result = DefaultHeader() + codeGenerator.ToString();

            GenerateFile(targetTypeName, result);
        }
    }
}