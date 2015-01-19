using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    internal sealed class MemoryTableInterface<TKey, TValue>
    {
        private readonly MemoryTable<TKey, TValue> physicalTable;

        public MemoryTableInterface(MemoryTable<TKey, TValue> table)
        {
            physicalTable = table;
        }

        public TValue Read(TKey key)
        {
            return physicalTable.Read(key);
        }

        public void Write(TKey key, TValue value)
        {
            physicalTable.Write(key,value);
        }

        public void Update(TKey key, TValue value)
        {
            physicalTable.Update(key, value);
        }

        public void Delete(TKey key)
        {
            physicalTable.Delete(key);
        }

    }
}
