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
        private readonly IMemoryTable<TKey, TValue> _table;

        public TableHandler(IMemoryTable<TKey, TValue> table)
        {
            Contract.Requires(table!=null);
            _table = table;
        }

        public IMemoryTable<TKey, TValue> Table { get { return _table; } }
        public override string TableName
        {
            get { return _table.TableName; }
        }

        public override bool IaValid(Type value)
        {
            return typeof (TValue) == value;
        }
    }
}
