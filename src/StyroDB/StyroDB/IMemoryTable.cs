using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    public interface IMemoryTable<in TKey, TValue>
    {
        string TableName { get; }

        int LockTimeout { get; set; }

        TValue Read(TKey key);

        bool Read(TKey key, out TValue result);

        void Write(TKey key, TValue value);

        void Delete(TKey key);

        IEnumerable<TValue> Query(Func<IEnumerable<TValue>, IEnumerable<TValue>> func);
    }
}
