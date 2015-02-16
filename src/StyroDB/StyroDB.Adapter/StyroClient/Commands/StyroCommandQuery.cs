using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    public class StyroCommandQuery<TKey, TValue> : StyroCommand
    {
        private readonly Func<IEnumerable<TValue>, IEnumerable<TValue>> _filter;

        public StyroCommandQuery(string tableName, Func<IEnumerable<TValue>, IEnumerable<TValue>> filter)
            : base(tableName)
        {
            Contract.Requires(filter!=null);
            _filter = filter;
        }

        public override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            var table = connection.GetTable<TKey, TValue>(tableName);
            var result = table.Query(_filter);

            return result.Cast<object>();
        }
    }
}
