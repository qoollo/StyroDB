using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Threading;

namespace StyroDB.InMemrory
{
    internal class MemoryTable<TKey, TValue> : BasicTable
    {
        private const int LockTimeout = 5000;

        protected readonly ReaderWriterLockSlim _cacheLock;
        protected readonly Dictionary<TKey, TValue> _innerCache;

        public MemoryTable(String tableName)
        {
            Contract.Requires(!String.IsNullOrEmpty(tableName));
            _name = tableName;

            _cacheLock = new ReaderWriterLockSlim();
            _innerCache = new Dictionary<TKey, TValue>();
        }

        public bool Read(TKey key, out TValue result, int timeout = LockTimeout)
        {
            CheckDisposed();
            if (_cacheLock.TryEnterReadLock(timeout))
            {
                try
                {
                    if (_innerCache.TryGetValue(key, out result))
                    {
                        return true;
                    }
                    else
                    {
                        result = default(TValue);
                        return false;
                    }
                }
                finally
                {
                    _cacheLock.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }

        public TValue Read(TKey key, int timeout = LockTimeout)
        {
            CheckDisposed();
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
            CheckDisposed();
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    TValue result;
                    if (_innerCache.TryGetValue(key, out result))
                    {
                        _innerCache[key] = value;
                    }
                    else
                    {
                        _innerCache.Add(key, value);
                    }
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

        public bool Delete(TKey key, int timeout = LockTimeout)
        {
            CheckDisposed();
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    if (_innerCache.ContainsKey(key))
                    {
                        _innerCache.Remove(key);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        public IEnumerable<TValue> Query(Func<IEnumerable<TValue>, IEnumerable<TValue>> func, int timeout = LockTimeout)
        {
            CheckDisposed();
            if (_cacheLock.TryEnterReadLock(timeout))
            {
                try
                {
                    return func(_innerCache.Values);
                }
                finally
                {
                    _cacheLock.ExitReadLock();
                }
            }
            throw new TimeoutException();
        } 

        protected override void Dispose(bool disposing)
        {
            disposed = true;
            if (_cacheLock != null) _cacheLock.Dispose();
            
        }
    }
}