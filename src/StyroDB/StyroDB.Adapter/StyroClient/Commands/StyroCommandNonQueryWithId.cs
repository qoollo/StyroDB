using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient.Commands
{
    internal class StyroCommandNonQueryWithId<TKey, TValue>:StyroCommand
    {
        private TKey _key;
        private readonly Action<TKey, IMemoryTable<TKey, TValue>> _action;

        public StyroCommandNonQueryWithId(string tableName, Action<TKey, IMemoryTable<TKey, TValue>> action)
            : base(tableName)
        {
            Contract.Requires(action!=null);            
            _action = action;
        }

        public void SetKey(TKey key)
        {
            _key = key;
        }

        internal override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            var table = connection.GetTable<TKey, TValue>(tableName);
            _action(_key, table);
        }

        internal override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
