using UnityEngine;

namespace CSVData {
    public interface ICSVListData {
        public virtual void PostProcessing() {}
    }
 
    public interface ICSVDictionaryData: ICSVListData {
     
        [field: SerializeField]
        public int SerialNumber { get;}
    }
   
}

