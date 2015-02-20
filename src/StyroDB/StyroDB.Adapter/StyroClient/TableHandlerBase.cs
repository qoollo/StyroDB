using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory;

namespace StyroDB.Adapter.StyroClient
{
    internal abstract class TableHandlerBase
    {
        public abstract string TableName { get; }

        public abstract bool IaValid(Type value);
    }
}
