using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    sealed class MemoryDatabase:IDisposable
    {
        private Dictionary<String, Object> _tables;

        public MemoryTableInterface<TKey, TValue> CreateTable<TKey, TValue>(String name)
        {
            dynamic newTable = new MemoryTable<TKey, TValue>(name);
            _tables.Add(name, newTable);
            return new MemoryTableInterface<TKey, TValue>(newTable);
        }

        public MemoryTableInterface<TKey, TValue> GetTable<TKey, TValue>(String name)
        {
            return new MemoryTableInterface<TKey,TValue>(_tables[name] as MemoryTable<TKey, TValue>);
        }

        public void DeleteTable(String name)
        {
            _tables.Remove(name);
        }

        public void Dispose()
        {
            // вызывать диспоз каждой таблицы
            //throw new NotImplementedException();
        }
    }
}
