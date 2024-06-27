using System.Collections;
    using System.Collections.Generic;

namespace WormComponents
    {
        public class CycledList<T> : IEnumerable<T> where T : class
        {
            private readonly LinkedList<T> _items = new();
            private LinkedListNode<T> _currentItem;
            
            public bool IsEmpty => _items.First == null;
            public int Count => _items.Count;
            
            public T Next()
            {
                if(_currentItem == _items.Last) 
                    _currentItem = _items.First;
                else if (_currentItem == null || _items.Contains(_currentItem.Value) == false)
                    _currentItem = _items.First;
                else
                    _currentItem = _currentItem.Next;
                    
                
                return _currentItem.Value;
            }
        
            public void Add(T item) => _items.AddLast(item);

            public void Remove(T item)
            {
                _items.Remove(item);
            }

            public void AddRange(IEnumerable<T> items)
            {
                foreach (var item in items)
                    _items.AddLast(item);
            }
            
            public IEnumerator<T> GetEnumerator()
            {
                return new CycledListEnumerator<T>(_items);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class CycledListEnumerator<T> : IEnumerator<T> where T : class
        {
            private readonly LinkedList<T> _list;
            private LinkedListNode<T> _currentItem;

            public object Current => _currentItem.Value;
            T IEnumerator<T>.Current => _currentItem.Value;

            public T CurrentObject => _currentItem.Value;
            
            public CycledListEnumerator(LinkedList<T> list)
            {
                _list = list;
            }

            public bool MoveNext()
            {
                _currentItem = _currentItem == null ? _list.First : _currentItem.Next;

                return _currentItem != null;
            }

            public void Reset() { }
            public void Dispose() { }
        }
    }