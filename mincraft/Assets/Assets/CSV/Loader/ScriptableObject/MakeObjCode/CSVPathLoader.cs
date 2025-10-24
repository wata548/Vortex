#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSVData.Extensions;
using DataType = System.String;

namespace CSVData {
    
    [CreateAssetMenu(menuName = "CSV/Loader/CSVByPath")]
    public class CSVPathLoader: LoaderBase {
        
        [Header("RawData: csv File path, TargetTypeName: type name(empty is recommanded)")]
        [SerializeField] private List<LoadInfo<DataType>> _infos;
        
        private List<List<string>> GetData(LoadInfo<DataType> loadInfo) {
            
            var isFirst = true;
            var csvData = new List<List<string>>();
            foreach (var rawData in loadInfo.RawDatas) {

                var tempPath = rawData;
                if (tempPath.IndexOf('.') == -1)
                    tempPath += ".csv";
                var tempRawData = File.ReadAllText(Path.Combine("Assets", tempPath));
                var tempData = CSV.Parse(tempRawData);

                //if is not first list element skip headers
                if (!isFirst) {
                    tempData = tempData
                        .Skip(2)
                        .ToList();
                }
                isFirst = false;
                    
                csvData.AddRange(tempData);
            }

            return csvData;
        }

        bool TempName(LoadInfo<DataType> info, out string tempName) {
            
            tempName = "";
            if (!string.IsNullOrWhiteSpace(info.TargetTypeName))
                return true;
            
            foreach (var candidate in info.RawDatas) {

                var fixCandidate = 
                    Path.GetFileNameWithoutExtension(candidate);
                if(string.IsNullOrWhiteSpace(fixCandidate)) 
                    continue;

                tempName = fixCandidate.FixTypeName();

                Debug.Log(tempName);
                
                break;
            }

            return !string.IsNullOrWhiteSpace(tempName); 
            
        }
        
        public override void Load(string directory) {

            foreach (var info in _infos) {

                if(!TempName(info, out var tempName)) 
                    continue;
                
                if (!info.SyncAble(Path.GetFileNameWithoutExtension(tempName))) {
                    continue;      
                }

                var csvData = GetData(info);
                SyncObject(info.TargetTypeName, csvData, directory);
            }
        }

        public override void Generate() {

            foreach (var info in _infos) {
                
                if(!TempName(info, out var tempName)) 
                    continue;
                
                if(!info.Generatable(Path.GetFileNameWithoutExtension(tempName)))
                    continue;

                bool needUpdate = !IsExistType(info.TargetTypeName);
                bool needUpdateTable = !IsExistType($"{info.TargetTypeName}Table");
                
                var csvData = GetData(info);
                CSV.GenerateObjCode(
                    datas:          csvData, 
                    targetTypeName: info.TargetTypeName, 
                    dataStyle:      info.csvDataStyle, 
                    makeType:       needUpdate,
                    makeTable:      needUpdateTable 
                );
            }
        }
    }
}

#endif