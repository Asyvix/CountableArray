using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace asyvix.ExNet.Core.Generic
{
    public class CountableArray<T> : IEnumerable<T>
    {
        private int _count;
        public int Count => _count;
        private CountableArrayItem<T>[] _items;
        public T[] Items
        {
            get
            {
                var returnArray = new T[_size];
                for (int i = 0; i < _size; i++)
                {
                    returnArray[i] = _items[i].Item;
                }

                return returnArray;
            }
        }

        private int _size;
        public int Length => _size;

        private const int MaxSize = 100000;

        public CountableArray(int size)
        {
            if(size > MaxSize)
                throw new Exception($"max index is {MaxSize}");
            if (size < 0)
                throw new ArgumentException();

            _items = new CountableArrayItem<T>[size];
            for (int i = 0; i < size; i++)
            {
                _items[i] = new CountableArrayItem<T>();
            }
            _size = size;
        }



        public T this[int index]
        {
            get
            {
                if (index >= _size)
                    throw new ArgumentException();

                return _items[index].IsAdd ? _items[index].Item : throw new NullReferenceException();
            }
            set
            {
                if (index > _items.Length)
                    throw new Exception("p1");

                if (!_items[index].IsAdd)
                {
                    _items[index].IsAdd = true;
                    _items[index].Item = value;
                    Counter(true);
                }
                else
                {
                    _items[index].Item = value;
                }
            }
        }

        public void Delete(int index)
        {
            if (index < 0)
                throw new ArgumentException();
            if (index > _items.Length)
                throw new ArgumentException();

            if (_items[index].IsAdd)
            {
                _items[index].Item = default(T);
                _items[index].IsAdd = false;
                Counter(false);
            }
        }
        private readonly object _lockCounter = new object();
        private void Counter(bool plus)
        {

            if (plus)
            {
                Interlocked.Increment(ref _count);
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }
            
        }

        #region [ IEnumerable Helper ]
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Items).GetEnumerator();
        }
        #endregion

    }

    internal class CountableArrayItem<T>
    {

        public bool IsAdd = false;
        public T Item;
    }
}
