using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyroDB.InMemrory.Exceptions
{
    public class TableAlreadyExistException : InMemoryException
    {
        public TableAlreadyExistException()
        {
        }

        public TableAlreadyExistException(string message)
            : base(message)
        {
        }

        public TableAlreadyExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
