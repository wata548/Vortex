using System;
using System.Collections;
using System.Collections.Generic;

namespace Extension {

    public enum SortType {
        Upper,
        Lower,
    }
    
    public class SortedList<T>: 
        IEnumerable<T>, 
        IEnumerable,
        IList<T>,
        IList,
        ICollection<T>,
        ICollection,
        IReadOnlyCollection<T>
        where T: IComparable
    {

        private List<T> _data;
        public readonly SortType SortType;

        public SortedList(SortType sortType = SortType.Upper) =>
            (_data, SortType) = (new(), sortType);

        public SortedList(List<T> init, SortType sortType = SortType.Upper) {
            _data = new();
            SortType = sortType;

            foreach (var value in init) {
                Add(value);
            }
        }
        
        public T this[Index idx] =>
            _data[idx];

        object IList.this[int index] {
            get => _data[index];
            set => throw new MethodAccessException("This List can't change value");
        }
        public T this[int index] {
            get => _data[index];
            set => throw new MethodAccessException("This List can't change value");
        }
        
        public void Add(T item) {

            if (_data.Count == 1) {
                
                var compare = _data[0].CompareTo(item);
                if (SortType == SortType.Lower)
                    compare *= -1;
                
                if (compare > 0)
                    _data.Insert(0, item);
                else 
                    _data.Add(item);
                return;
            }
            
            int start = 0;
            int end = _data.Count - 1;
            while (start <= end) {
                var middle = (start + end) >> 1;
                var compare = _data[middle].CompareTo(item);
                if (SortType == SortType.Lower)
                    compare *= -1;
                
                if (compare > 0) {

                    if (start == end)
                        break;
                    
                    end = middle;
                }
                else if (compare < 0) {
                    start = middle + 1;
                }
                else {
                    start = middle;
                    break;
                }
            }
            _data.Insert(start, item);
        }

        public int Add(object value) =>
            throw new MethodAccessException("This List can't change value");

        public void Clear() =>
            _data.Clear();

        public bool Contains(object value) {
            
            if(value is T v)
               return _data.Contains(v);
            return false;
        }

        public int IndexOf(object value) {
            if(value is T v)
               return _data.IndexOf(v);
            return -1;
        }

        public void Insert(int index, object value) {
            throw new MethodAccessException("This List can't change value");
        }

        public void Remove(object value) {
            if (value is T v)
                _data.Remove(v);
        }

        public bool Contains(T item) =>
            _data.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            _data.CopyTo(array, arrayIndex);

        bool ICollection<T>.Remove(T item) =>
            _data.Remove(item);

        public void CopyTo(Array array, int index) =>
            _data.CopyTo((T[])array, index);

        public int Count => _data.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        public bool IsReadOnly => false;

        public void Remove(T value) =>
            _data.Remove(value);
        
        
        
        public IEnumerator<T> GetEnumerator() =>
            _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int IndexOf(T item) =>
            _data.IndexOf(item);

        public void Insert(int index, T item) {
            throw new MethodAccessException("This List can't change value");
        }

        public void RemoveAt(int index) {
            _data.RemoveAt(index);
        }

        public bool IsFixedSize => false;
    }
}