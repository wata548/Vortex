using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVData.Extensions {

    [Serializable]
    public class SerializablePair<K, V> {
        public K Key;
        public V Value;

        public SerializablePair(K key, V value) {
            Key = key;
            Value = value;
        }

        public SerializablePair(System.Collections.Generic.KeyValuePair<K, V> row) {
            Key = row.Key;
            Value = row.Value;
        }
    }
    
    [Serializable]
    public class SerializableDictionary<K, V>: Dictionary<K, V>, ISerializationCallbackReceiver {

        [SerializeField] private List<SerializablePair<K, V>> _dataList;

        public SerializableDictionary() =>
            _dataList = new();
        
        public SerializableDictionary(Dictionary<K, V> datas) {

            _dataList = new();
            foreach (var data in datas) {
                
                _dataList.Add(new (data));
                Add(data.Key, data.Value);
            }
        }

        public void OnBeforeSerialize() {
            _dataList.Clear();

            foreach (var data in this)
                _dataList.Add(new(data));
        }

        public void OnAfterDeserialize() {
            this.Clear();

            foreach (var data in _dataList)
                this.Add(data.Key, data.Value);
        }
    }
}