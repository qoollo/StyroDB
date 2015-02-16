﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient.Commands
{
    public class StyroCommandNonQuery<TKey, TValue>:StyroCommand
    {
        private readonly Action<MemoryTableInterface<TKey, TValue>> _action;

        public StyroCommandNonQuery(string tableName, 
            Action<MemoryTableInterface<TKey, TValue>> action ) : base(tableName)
        {
            Contract.Requires(action!=null);
            _action = action;
        }

        public override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            var table = connection.GetTable<TKey, TValue>(tableName);
            _action(table);
        }

        public override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
