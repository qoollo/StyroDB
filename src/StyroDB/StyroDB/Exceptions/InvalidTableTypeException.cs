using System;

namespace StyroDB.InMemrory.Exceptions
{
    internal class InvalidTableTypeException : InMemoryException
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