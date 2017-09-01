using System;

namespace Astral.Exceptions
{
    public class EncodingException : PermanentException
    {
        public EncodingException()
        {
        }

        public EncodingException(string message) : base(message)
        {
        }

        public EncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}