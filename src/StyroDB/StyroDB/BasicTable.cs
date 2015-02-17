using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    abstract class BasicTable: IDisposable
    {
        private readonly String _name;
        private bool _disposed = false;

        public string TableName { get { return _name; } }

        protected BasicTable(String tableName)
        {
            Contract.Requires(!String.IsNullOrEmpty(tableName));
            _name = tableName;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        protected void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Object already disposed.");
            }
        }
    }
}
