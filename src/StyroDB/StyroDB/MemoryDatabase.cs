using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    internal sealed class MemoryDatabase
    {
        private Dictionary<String, Object> tables;

        public MemoryTableInterface<TKey, TValue> CreateTable<TKey, TValue>(String name)
        {
            dynamic newTable = new MemoryTable<TKey, TValue>(name);
            tables.Add(name, newTable);
            return new MemoryTableInterface<TKey, TValue>(newTable);
        }

        public MemoryTableInterface<TKey, TValue> GetTable<TKey, TValue>(String name)
        {
            return new MemoryTableInterface<TKey,TValue>(tables[name] as MemoryTable<TKey, TValue>);
        }

        public void DeleteTable(String name)
        {
            tables.Remove(name);
        }
    }
}
