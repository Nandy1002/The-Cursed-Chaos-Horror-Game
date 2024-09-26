using System;

namespace VladislavTsurikov.ObjectPool.Scripts
{
    public readonly struct PooledObject<T> : IDisposable where T : class
    {
        private readonly T _toReturn;
        private readonly IObjectPool<T> _pool;

        internal PooledObject(T value, IObjectPool<T> pool)
        {
            _toReturn = value;
            _pool = pool;
        }

        void IDisposable.Dispose() => _pool.Release(_toReturn);
    }
}