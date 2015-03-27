using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.Wrappers
{
    internal class MemoryTableWrapper<TKey, TValue> : IMemoryTable<TKey, TValue>
    {
        private readonly IMemoryTable<TKey, ValueWrapper<TKey, TValue>> _table;

        public MemoryTableWrapper(IMemoryTable<TKey, ValueWrapper<TKey, TValue>> table)
        {
            Contract.Requires(table != null);
            _table = table;
        }

        #region IMemoryTable<TKey, TValue>

        public string TableName
        {
            get { return _table.TableName; }
        }

        public int LockTimeout
        {
            get { return _table.LockTimeout; }
            set { _table.LockTimeout = value; }
        }

        public TValue Read(TKey key)
        {
            return _table.Read(key).Value;
        }

        public bool Read(TKey key, out TValue result)
        {
            ValueWrapper<TKey, TValue> value;

            var ret = _table.Read(key, out value);
            result = value == null ? default(TValue) : value.Value;

            return ret;
        }

        public void Write(TKey key, TValue value)
        {
            _table.Write(key, new ValueWrapper<TKey, TValue>(key, value));
        }

        public void Delete(TKey key)
        {
            _table.Delete(key);
        }

        public IEnumerable<TValue> Query(Func<IEnumerable<TValue>, IEnumerable<TValue>> func)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMemoryTable<TKey, ValueWrapper<TValue>>

        public ValueWrapper<TKey, TValue> ReadWrapper(TKey key)
        {
            return _table.Read(key);
        }

        public bool Read(TKey key, out ValueWrapper<TKey, TValue> result)
        {
            return _table.Read(key, out result);
        }

        public void Write(TKey key, ValueWrapper<TKey, TValue> value)
        {
            _table.Write(key, value);
        }

        public IEnumerable<ValueWrapper<TKey, TValue>> Query(
            Func<IEnumerable<ValueWrapper<TKey, TValue>>, IEnumerable<ValueWrapper<TKey, TValue>>> func)
        {
            return _table.Query(func);
        }

        #endregion
    }
}
