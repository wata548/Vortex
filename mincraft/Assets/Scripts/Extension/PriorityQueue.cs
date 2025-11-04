using System;
using System.Collections;
using System.Collections.Generic;

namespace Extension {
    
    public class PriorityQueue<T>: IEnumerable<T>, ICollection<T> where T: IComparable<T> {
        private bool _isReverse;
        
        private List<T> _datas;
        
        
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }

        int ICollection<T>.Count => _datas.Count;
        
        public bool IsReadOnly { get; }

        public PriorityQueue(bool pIsReverse = false) {
            _isReverse = pIsReverse;
            _datas = new();
        }

        private void ChangeParent(int idx = -1) {
            if (idx == -1) idx = _datas.Count - 1;
            
            while (true) {
                var parent = idx / 2;
                if (_isReverse != (_datas[parent].CompareTo(_datas[idx]) > 0)) 
                    return;

                (_datas[idx], _datas[parent]) = (_datas[parent], _datas[idx]);
                idx = parent;
                if (idx == 0)
                    return;
            }
        }

        public T Pop() {
            var result = _datas[0];
            (_datas[0], _datas[^1]) = (_datas[^1], _datas[0]);
            _datas.RemoveAt(_datas.Count - 1);

            var idx = 0;
            while (true) {

                if (_datas.Count <= idx * 2)
                    break;
                
                var son1 = _datas[idx * 2];
                if (_datas.Count <= idx * 2 + 1) {
                    if (_isReverse == son1.CompareTo(_datas[idx]) < 0)
                        (_datas[idx], _datas[idx * 2]) = (_datas[idx * 2], _datas[idx]);
                    break;
                }
                
                var son2 = _datas[idx * 2 + 1];

                if (_isReverse == son1.CompareTo(son2) > 0) {
                    if (_isReverse == son2.CompareTo(_datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2 + 1]) = (_datas[idx * 2 + 1], _datas[idx]);
                    idx = 2 * idx + 1;
                }
                else {
                    if (_isReverse == son1.CompareTo(_datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2]) = (_datas[idx * 2], _datas[idx]);
                    idx *= 2;
                }
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator() => 
            _datas.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _datas.GetEnumerator();

        public void Add(T item) {
            _datas.Add(item);
            ChangeParent();
        }

        public void Clear() =>
            _datas.Clear();

        public bool Contains(T item) =>
            _datas.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            _datas.CopyTo(array, arrayIndex);

        public bool Remove(T item) {
            if (!_datas.Contains(item))
                return false;
            
            var idx = _datas.IndexOf(item);
            (_datas[idx], _datas[^1]) = (_datas[^1], _datas[idx]);
            _datas.RemoveAt(_datas.Count - 1);
            
            while (true) {

                if (_datas.Count <= idx * 2)
                    break;
                
                var son1 = _datas[idx * 2];
                if (_datas.Count <= idx * 2 + 1) {
                    if (_isReverse == son1.CompareTo(_datas[idx]) < 0)
                        (_datas[idx], _datas[idx * 2]) = (_datas[idx * 2], _datas[idx]);
                    break;
                }
                
                var son2 = _datas[idx * 2 + 1];

                if (_isReverse == son1.CompareTo(son2) > 0) {
                    if (_isReverse == son2.CompareTo(_datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2 + 1]) = (_datas[idx * 2 + 1], _datas[idx]);
                    idx = 2 * idx + 1;
                }
                else {
                    if (_isReverse == son1.CompareTo(_datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2]) = (_datas[idx * 2], _datas[idx]);
                    idx *= 2;
                }
            }           
            return true;
        }

    }
}