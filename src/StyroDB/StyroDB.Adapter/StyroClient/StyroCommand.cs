using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace StyroDB.Adapter.StyroClient
{
    public abstract class StyroCommand
    {
        private readonly string _tableName;
        public StyroConnection Connection { get; set; }

        protected StyroCommand(string tableName)
        {
            Contract.Requires(!string.IsNullOrEmpty(tableName));
            _tableName = tableName;            
        }

        public void ExecuteNonQuery()
        {
            ExecuteNonQueryInner(Connection, _tableName);
        }

        public IEnumerable<object> ExecuteReader()
        {
            return ExecuteReaderInner(Connection, _tableName);
        }

        public abstract void ExecuteNonQueryInner(StyroConnection connection, string tableName);

        public abstract IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName);
    }
}
