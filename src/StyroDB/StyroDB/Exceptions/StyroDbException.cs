using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory.Exceptions
{
    public class StyroDbException: Exception
    {
        public StyroDbException()
        {
        }

        public StyroDbException(string message)
            : base(message)
        {
        }

        public StyroDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
