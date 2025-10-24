#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using CSVData.Extensions;

namespace CSVData {

    [CreateAssetMenu(menuName = "CSV/Loader/Generator/Path")]
    public class CSVPathCodeGenerator : CodeGenerateLoaderBase {
        
        [Header("Key: CSV File path, Value: type name(empty is recommanded)")]
        [SerializeField] private List<SerializablePair<string, string>> _infos;
        
        public override void Load(string directory) {}

        public override void Generate() {
            
            foreach (var sheet in _infos) {
                
                if(string.IsNullOrWhiteSpace(sheet.Key))
                    continue;

                if (sheet.Key.IndexOf('.') == -1)
                    sheet.Key += ".csv";

                if (string.IsNullOrWhiteSpace(sheet.Value))
                    sheet.Value = Path.GetFileNameWithoutExtension(sheet.Key);

                sheet.Value = sheet.Value.FixTypeName();


                var rawData = File.ReadAllText(Path.Combine("Assets", sheet.Key));
                var csvData = CSV.Parse(rawData);
                CSV.GenerateCode(sheet.Value, csvData);
            }
        }
    }
}

#endif