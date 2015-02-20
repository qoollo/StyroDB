using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory.Exceptions;

namespace StyroDB.InMemrory
{
    public class MemoryDatabase : IMemoryDatabase, IDisposable
    {
        private readonly Dictionary<String, BasicTable> _tables;
        private bool _disposed;

        public MemoryDatabase()
        {
            _tables = new Dictionary<string, BasicTable>();
            _disposed = false;
        }

        public IMemoryTable<TKey, TValue> CreateTable<TKey, TValue>(String name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));

            CheckDisposed();
            if (_tables.ContainsKey(name))
            {
                throw new TableAlreadyExistException(String.Format("Table {0} already exist in database and can't be overwritten.", name));
            }

            var newTable = new MemoryTable<TKey, TValue>(name);
            _tables.Add(name, newTable);
            return new MemoryTableInterface<TKey, TValue>(newTable);
        }

        public IMemoryTable<TKey, TValue> GetTable<TKey, TValue>(String name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));

            CheckDisposed();
            if (!_tables.ContainsKey(name))
            {
                return null;
            }
            
            try
            {
                var table = _tables[name];
                var table2 = table as MemoryTable<TKey, TValue>;

                var memTable = new MemoryTableInterface<TKey, TValue>((MemoryTable<TKey, TValue>)_tables[name]);
                return memTable;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidTableTypeException(String.Format("Table {0} couldn't cast to requested type", name),ex);
            }
        }

        public void DeleteTable(String name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            CheckDisposed();
            if (_tables.ContainsKey(name))
            { // TODO: возможна ошибка, когда 2 потока пытаются вызвать этот метод
                _tables[name].Dispose(); 
                _tables.Remove(name);
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Object already disposed.");
            }
        }

        public void Dispose()
        {
            _disposed = true;
            foreach (var table in _tables)
            {
                table.Value.Dispose();
            }
            
        }
    }
}
