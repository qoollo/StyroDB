using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.Adapter.StyroClient.Commands
{
    public class StyroCommandDatabaseAction:StyroCommand
    {
        private readonly Action<StyroConnection> _action;

        public StyroCommandDatabaseAction(Action<StyroConnection> action) : base("-1")
        {
            Contract.Requires(action!=null);
            _action = action;
        }

        internal override void ExecuteNonQueryInner(StyroConnection connection, string tableName)
        {
            _action(connection);
        }

        internal override IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
