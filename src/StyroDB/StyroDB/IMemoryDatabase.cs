using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    public interface IMemoryDatabase:IDisposable
    {
        MemoryTableInterface<TKey, TValue> CreateTable<TKey, TValue>(String name);

        MemoryTableInterface<TKey, TValue> GetTable<TKey, TValue>(String name);

        void DeleteTable(String name);
    }
}
