﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.Wrappers
{
    internal class MemoryDatabaseWrapper : IMemoryDatabase
    {
        private readonly IMemoryDatabase _memoryDatabase;

        public MemoryDatabaseWrapper(IMemoryDatabase memoryDatabase)
        {
            Contract.Requires(memoryDatabase != null);
            _memoryDatabase = memoryDatabase;
        }

        public void Dispose()
        {
            _memoryDatabase.Dispose();
        }

        public IMemoryTable<TKey, TValue> CreateTable<TKey, TValue>(string name)
        {
            IMemoryTable<TKey, ValueWrapper<TKey, TValue>> table =
                _memoryDatabase.CreateTable<TKey, ValueWrapper<TKey, TValue>>(name);

            return new MemoryTableWrapper<TKey, TValue>(table);
        }

        public IMemoryTable<TKey, TValue> GetTable<TKey, TValue>(string name)
        {
            return new MemoryTableWrapper<TKey, TValue>(_memoryDatabase.GetTable<TKey, ValueWrapper<TKey, TValue>>(name));
        }

        public void DeleteTable(string name)
        {
            _memoryDatabase.DeleteTable(name);
        }
    }
}
