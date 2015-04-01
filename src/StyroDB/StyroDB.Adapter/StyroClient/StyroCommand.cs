using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace StyroDB.Adapter.StyroClient
{
    internal class StyroCommand
    {
        private readonly string _tableName;
        public StyroConnection Connection { get; set; }

        public Func<StyroConnection, string, IEnumerable<object>> ExecuteReaderInner;
        public Action<StyroConnection, string> ExecuteNonQueryInner;
        public Func<IEnumerable<object>, StyroDataReader> ExecuteReaderFunc;

        public StyroCommand(string tableName)
        {
            Contract.Requires(!string.IsNullOrEmpty(tableName));
            _tableName = tableName;
            ExecuteNonQueryInner = (connection, s) => { throw new NotImplementedException(); };
            ExecuteReaderInner = (connection, s) => { throw new NotImplementedException(); };
            ExecuteReaderFunc = objects => new StyroDataReader(objects);
        }

        public void ExecuteNonQuery()
        {
            ExecuteNonQueryInner(Connection, _tableName);
        }

        public StyroDataReader ExecuteReader()
        {
            return ExecuteReaderFunc(ExecuteReaderInner(Connection, _tableName));
        }

        public IEnumerable<object> ExecuteReaderGetCollection()
        {
            return ExecuteReaderInner(Connection, _tableName);
        }
    }
}
