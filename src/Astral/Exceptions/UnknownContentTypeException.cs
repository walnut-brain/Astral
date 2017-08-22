using System;

namespace Astral.Exceptions
{
    public class UnknownContentTypeException : Exception
    {
        public UnknownContentTypeException()
        {
        }

        public UnknownContentTypeException(string message) : base(message)
        {
        }

        public UnknownContentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}