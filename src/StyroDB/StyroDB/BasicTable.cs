using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory
{
    class BasicTable: IDisposable
    {
        protected String _name;
        protected bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        protected void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Object already disposed.");
            }
        }
    }
}
