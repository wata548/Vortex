using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Extension {
    
    public class PriorityQueue<T>: IEnumerable<T>, ICollection<T> {
        private bool _isReverse;
        
        private List<T> _datas;
        private Func<T, T, int> _comparer;

        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public int Count => _datas.Count;
        int ICollection<T>.Count => _datas.Count;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Comp(T pLhs, T pRhs) {
            if (_comparer != null) return _comparer!.Invoke(pLhs, pRhs);
            if (pLhs is IComparable<T> genericComp) return genericComp.CompareTo(pRhs);
            if (pLhs is IComparable comp) return comp.CompareTo(pRhs);
            throw new ArgumentException("Add comparer");           
        }
        
        public PriorityQueue(bool pIsReverse = false, Func<T, T, int> pCompare = null) {
            _isReverse = pIsReverse;
            _datas = new();
            _comparer = pCompare;
        }

        private void ChangeParent(int idx = -1) {
            if (idx == -1) idx = _datas.Count - 1;
            
            while (true) {
                var parent = (idx - 1) / 2;
                if (_isReverse != Comp(_datas[parent], _datas[idx]) > 0) 
                    return;

                (_datas[idx], _datas[parent]) = (_datas[parent], _datas[idx]);
                idx = parent;
                if (idx == 0)
                    return;
            }
        }

        public T Dequeue() {
            var result = _datas[0];
            (_datas[0], _datas[^1]) = (_datas[^1], _datas[0]);
            _datas.RemoveAt(_datas.Count - 1);

            var idx = 0;
            while (true) {

                if (_datas.Count <= idx * 2 + 1)
                    break;
                
                var son1 = _datas[idx * 2 + 1];
                if (_datas.Count <= idx * 2 + 2) {
                    if (_isReverse == Comp(son1, _datas[idx]) < 0)
                        (_datas[idx], _datas[idx * 2 + 1]) = (_datas[idx * 2 + 1], _datas[idx]);
                    break;
                }
                
                var son2 = _datas[idx * 2 + 2];
                if (_isReverse == Comp(son1, son2) > 0) {
                    
                    if (_isReverse == Comp(son2, _datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2 + 2]) = (_datas[idx * 2 + 2], _datas[idx]);
                    idx = 2 * idx + 2;
                }
                else {
                    if (_isReverse == Comp(son1, _datas[idx]) > 0)
                        break;
                    (_datas[idx], _datas[idx * 2 + 1]) = (_datas[idx * 2 + 1], _datas[idx]);
                    idx = 2 * idx + 1;
                }
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator() => 
            _datas.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _datas.GetEnumerator();

        public void Enqueue(T pItem) {
            _datas.Add(pItem);
            ChangeParent();
        }

        public void Add(T pItem) => 
            Enqueue(pItem);

        public void Clear() =>
            _datas.Clear();

        public bool Contains(T item) =>
            _datas.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            _datas.CopyTo(array, arrayIndex);

        public bool Remove(T item) {
            throw new InvalidOperationException("Sorry, Priority Queue can't remove element.");
        }

    }
}