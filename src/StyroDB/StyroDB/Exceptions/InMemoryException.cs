using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory.Exceptions
{
    public class InMemoryException : StyroDbException
    {
        public InMemoryException()
        {
        }

        public InMemoryException(string message)
            : base(message)
        {
        }

        public InMemoryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
