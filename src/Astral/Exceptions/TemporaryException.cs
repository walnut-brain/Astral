using System;

namespace Astral.Exceptions
{
    public class TemporaryException : Exception
    {
        public TemporaryException()
        {
        }

        public TemporaryException(string message) : base(message)
        {
        }

        public TemporaryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}