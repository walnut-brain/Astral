using System;

namespace Astral
{
    public class UnknownContentTypeException : PermanentException
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