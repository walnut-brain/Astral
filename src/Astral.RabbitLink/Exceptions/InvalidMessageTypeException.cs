using System;

namespace Astral.RabbitLink.Exceptions
{
    public class InvalidMessageTypeException : Exception
    {
        public InvalidMessageTypeException()
        {
        }

        public InvalidMessageTypeException(string message) : base(message)
        {
        }

        public InvalidMessageTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}