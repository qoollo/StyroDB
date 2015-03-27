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

        public StyroDataReader ExecuteReader()
        {
            return new StyroDataReader(ExecuteReaderInner(Connection, _tableName));
        }

        internal abstract void ExecuteNonQueryInner(StyroConnection connection, string tableName);

        internal abstract IEnumerable<object> ExecuteReaderInner(StyroConnection connection, string tableName);
    }
}
