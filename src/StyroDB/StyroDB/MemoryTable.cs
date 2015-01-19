using System;
using System.Collections.Generic;
using System.Threading;

namespace StyroDB.InMemrory
{
    internal sealed class MemoryTable<TKey, TValue>
    {
        private const int LockTimeout = 5000;
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _innerCache = new Dictionary<TKey, TValue>();
        private String name;

        public MemoryTable(String tableName)
        {
            name = tableName;
        }

        public TValue Read(TKey key, int timeout = LockTimeout)
        {
            if (_cacheLock.TryEnterReadLock(timeout))
            {
                try
                {
                    return _innerCache[key];
                }
                finally
                {
                    _cacheLock.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }


        public void Write(TKey key, TValue value, int timeout = LockTimeout)
        {
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    _innerCache.Add(key, value);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
            else
            {
                throw new TimeoutException();
            }
        }

        public void Delete(TKey key, int timeout = LockTimeout)
        {
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    _innerCache.Remove(key);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
            else
            {
                throw new TimeoutException();
            }
        }

        public void Update(TKey key, TValue value, int timeout = LockTimeout)
        {
            if (_cacheLock.TryEnterUpgradeableReadLock(timeout))
            {
                try
                {
                    UpdateUnderLock(key, value, timeout);
                }
                finally
                {
                    _cacheLock.ExitUpgradeableReadLock();
                }
            }
            else
            {
                throw new TimeoutException();
            }
        }

        private void UpdateUnderLock(TKey key, TValue value, int timeout = LockTimeout)
        {
            TValue result;
            if (_innerCache.TryGetValue(key, out result))
            {
                if (_cacheLock.TryEnterWriteLock(timeout))
                {
                    try
                    {
                        _innerCache[key] = value;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            else
            {
                Write(key, value, timeout);
            }
        }

        ~MemoryTable()
        {
            if (_cacheLock != null) _cacheLock.Dispose();
        }
    }
}