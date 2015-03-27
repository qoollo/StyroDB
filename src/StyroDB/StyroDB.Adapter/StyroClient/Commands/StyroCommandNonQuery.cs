using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient.Commands
{
    public class StyroCommandNonQuery<TKey, TValue>:StyroCommand
    {
        private readonly Action<IMemoryTable<TKey, TValue>> _action;

        public StyroCommandNonQuery(string tableName, Action<IMemoryTable<TKey, TValue>> action)
            : base(tableName)
        {
            Contract.Requires(action!=null);
            _action = action;
        }

        internal override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            var table = connection.GetTable<TKey, TValue>(tableName);
            _action(table);
        }

        internal override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
