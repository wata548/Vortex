#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.Net.Http;
using CSVData.Extensions;

namespace CSVData {

    [CreateAssetMenu(menuName = "CSV/Loader/Generator/SpreadSheet")]
    public class SpreadSheetCodeGenerator : CodeGenerateLoaderBase {
        
        [SerializeField] private string _apiKey;
        [Header(@"In spreadSheet(<size=70%>'https://docs.google.com/spreadsheets/d/XXX/edit?gid=YYY#gid=ZZZ'</size>) link, XXX is path")]
        [SerializeField] private string _path;
        [Header("Key: sheet(page) name, Value: type name(empty is recommanded)")]
        [SerializeField] private List<SerializablePair<string, string>> _infos;
        
        public override void Load(string directory) {}

        public override void Generate() {
            
            var httpClient = new HttpClient();
            
            foreach (var sheet in _infos) {
                
                if(string.IsNullOrWhiteSpace(sheet.Key))
                    continue;

                if (string.IsNullOrWhiteSpace(sheet.Value))
                    sheet.Value = sheet.Key;

                sheet.Value = sheet.Value.FixTypeName();

                var data = SpreadSheet.LoadData(_path, sheet.Key, _apiKey);

                CSV.GenerateCode(sheet.Value, data);
            }
        }
    }
}

#endif