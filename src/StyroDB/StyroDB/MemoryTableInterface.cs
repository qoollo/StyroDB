using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    public class MemoryTableInterface<TKey, TValue>
    {
        private readonly MemoryTable<TKey, TValue> physicalTable;
        private int _lockTimeout = 5000; // TODO: public property

        internal MemoryTableInterface(MemoryTable<TKey, TValue> table)
        {
            physicalTable = table;
        }

        public int LockTimeout
        {
            get { return _lockTimeout; }
            set { _lockTimeout = value; }
        }

        public TValue Read(TKey key)
        {
            return physicalTable.Read(key, _lockTimeout);
        }

        public void Write(TKey key, TValue value)
        {
            physicalTable.Write(key, value, _lockTimeout);
        }

        public void Update(TKey key, TValue value)
        {
            physicalTable.Update(key, value, _lockTimeout);
        }

        public void Delete(TKey key)
        {
            physicalTable.Delete(key, _lockTimeout);
        }

    }
}
