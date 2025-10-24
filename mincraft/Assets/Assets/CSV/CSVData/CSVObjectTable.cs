using CSVData.Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace CSVData {
    
    public abstract class CSVObjectTable: ScriptableObject {}
    
    public abstract class CSVDictionaryTable<T>: CSVObjectTable where T: ICSVDictionaryData { 
        [field: SerializeField]
        public SerializableDictionary<int, T> Table { get; protected set; }

    }
    
    public abstract class CSVListTable<T>: CSVObjectTable where T: ICSVListData { 
        [field: SerializeField] 
        public List<T> Table { get; protected set; }
    }
}