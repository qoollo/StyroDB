using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient
{
    public class StyroConnection:IDisposable
    {
        private readonly IMemoryDatabase _connection;
        private readonly List<TableHandlerBase> _tables;
        private readonly ReaderWriterLockSlim _lock;

        public StyroConnection(IMemoryDatabase connection)
        {
            Contract.Requires(connection != null);
            _connection = connection;
            _tables = new List<TableHandlerBase>();
            _lock = new ReaderWriterLockSlim();
        }

        public IMemoryTable<TKey, TValue> GetTable<TKey, TValue>(string tableName)
        {
            return ReadTableFromLocal<TKey, TValue>(tableName) 
                ?? CreateOrReadTable<TKey, TValue>(tableName);
        }

        private IMemoryTable<TKey, TValue> ReadTableFromLocal<TKey, TValue>(string tableName)
        {
            IMemoryTable<TKey, TValue> ret = null;

            _lock.EnterReadLock();
            try
            {
                ret = ReadTableFromLocalNoLock<TKey, TValue>(tableName);
            }
            finally
            {
                _lock.ExitReadLock();
            }
            return ret;
        }

        private IMemoryTable<TKey, TValue> ReadTableFromLocalNoLock<TKey, TValue>(string tableName)
        {
            IMemoryTable<TKey, TValue> ret = null;
            foreach (var table in _tables)
            {
                if (string.Equals(table.TableName, tableName))
                {
                    ret = ((TableHandler<TKey, TValue>) table).Table;
                    break;
                }
            }
            return ret;
        }

        private IMemoryTable<TKey, TValue> CreateOrReadTable<TKey, TValue>(string tableName)
        {
            IMemoryTable<TKey, TValue> ret = null;

            _lock.EnterWriteLock();
            try
            {
                ret = ReadTableFromLocalNoLock<TKey, TValue>(tableName);

                if (ret == null)
                {
                    ret = _connection.GetTable<TKey, TValue>(tableName);
                    if(ret!=null)
                        _tables.Add(new TableHandler<TKey, TValue>(ret));
                }    
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            return ret;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
