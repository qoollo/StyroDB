using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient
{
    internal class TableHandler<TKey, TValue>:TableHandlerBase
    {
        private readonly MemoryTableInterface<TKey, TValue> _table;

        public TableHandler(MemoryTableInterface<TKey, TValue> table)
        {
            Contract.Requires(table!=null);
            _table = table;
        }

        public MemoryTableInterface<TKey, TValue> Table { get { return _table; } }
        public override string TableName
        {
            get { return _table.TableName; }
        }
    }
}
