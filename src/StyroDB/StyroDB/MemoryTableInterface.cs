using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    public class MemoryTableInterface<TKey, TValue>
    {
        private readonly MemoryTable<TKey, TValue> _physicalTable;
        private int _lockTimeout = 5000; 

        internal MemoryTableInterface(MemoryTable<TKey, TValue> table)
        {
            Contract.Requires(table !=  null);
            _physicalTable = table;
        }

        public string TableName { get { return _physicalTable.TableName; } }
        public int LockTimeout
        {
            get { return _lockTimeout; }
            set
            {
                Contract.Requires(value > 0 || value == -1);
                _lockTimeout = value;
            }
        }

        public TValue Read(TKey key)
        {
            return _physicalTable.Read(key, _lockTimeout);
        }

        public bool Read(TKey key, out TValue result)
        {
            var exist = _physicalTable.Read(key, out result, _lockTimeout);
            return exist;
        }

        public void Write(TKey key, TValue value)
        {
            _physicalTable.Write(key, value, _lockTimeout);
        }

        public void Delete(TKey key)
        {
            _physicalTable.Delete(key, _lockTimeout);
        }

        public IEnumerable<TValue> Query(Func<IEnumerable<TValue>, IEnumerable<TValue>> func)
        {
            return _physicalTable.Query(func, _lockTimeout);
        }

    }
}
