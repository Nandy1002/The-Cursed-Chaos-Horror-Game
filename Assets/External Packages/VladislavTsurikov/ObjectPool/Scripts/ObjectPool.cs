using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ObjectPool.Scripts
{
    public abstract class ObjectPool<T> : IObjectPool<T> where T : class
    {
        internal readonly Stack<T> Stack;
        private readonly int _maxSize;
        internal bool _collectionCheck;
        private bool _countLimitation;
        
        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => Stack.Count;

        protected ObjectPool(bool collectionCheck = false)
        {
            Stack = new Stack<T>();
            _collectionCheck = collectionCheck;
            _countLimitation = false;
        }

        protected ObjectPool(bool collectionCheck = false, int defaultCapacity = 10, int maxSize = 10000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Max Size must be greater than 0", nameof (maxSize));
            Stack = new Stack<T>(defaultCapacity);
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
            _countLimitation = false;
        }

        protected abstract T OnCreateObject();
        protected virtual void OnGet(T go){}
        protected virtual void OnRelease(T go){}
        protected virtual void OnDestroy(T element){}

        public T Get()
        {
            T obj;
            if (Stack.Count == 0)
            {
                obj = OnCreateObject();
                ++CountAll;
            }
            else
                obj = Stack.Pop();

            OnGet(obj);
            return obj;
        }

        public PooledObject<T> Get(out T v) => new PooledObject<T>(v = Get(), (IObjectPool<T>) this);

        public void Release(T element)
        {
            if (_collectionCheck && Stack.Count > 0 && Stack.Contains(element))
                throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
            OnRelease(element);
            
            if (!_countLimitation || CountInactive < _maxSize)
            {
                Stack.Push(element);
            }
            else
            {
                OnDestroy(element);
            }
        }

        public void Clear()
        {
            foreach (var obj in Stack) OnDestroy(obj);
            
            Stack.Clear();
            CountAll = 0;
        }

        public void InternalDispose()
        {
            Dispose();
            Clear();
        }

        protected virtual void Dispose() {}
    }
}