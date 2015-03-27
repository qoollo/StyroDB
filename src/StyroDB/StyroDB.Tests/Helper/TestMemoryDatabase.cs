using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Tests.Helper
{
    class TestMemoryDatabase : IMemoryDatabase
    {
        private readonly MemoryDatabase _database;
        public int GetCount = 0;

        public TestMemoryDatabase(MemoryDatabase database)
        {
            _database = database;
        }

        public IMemoryTable<TKey, TValue> CreateTable<TKey, TValue>(String name)
        {            
            return _database.CreateTable<TKey, TValue>(name);
        }

        public IMemoryTable<TKey, TValue> GetTable<TKey, TValue>(string name)
        {
            GetCount++;
            return _database.GetTable<TKey, TValue>(name);
        }

        public void DeleteTable(string name)
        {
            _database.DeleteTable(name);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
