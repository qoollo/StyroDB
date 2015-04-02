using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyroDB.InMemrory.Exceptions;

namespace StyroDB.Adapter.Converter.Exceptions
{
    class StyroQueryException: StyroDbException
    {
        public StyroQueryException()
        {
        }

        public StyroQueryException(string message)
            : base(message)
        {
        }

        public StyroQueryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
