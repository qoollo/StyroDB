using System;

namespace StyroDB.InMemrory.Exceptions
{
    public class InvalidTableTypeException : InMemoryException
    {
        public InvalidTableTypeException()
        {
        }

        public InvalidTableTypeException(string message)
            : base(message)
        {
        }

        public InvalidTableTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}