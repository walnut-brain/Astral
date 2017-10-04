using System;

namespace Astral.Payloads
{
    public class UnknownContentTypeException : PayloadException
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