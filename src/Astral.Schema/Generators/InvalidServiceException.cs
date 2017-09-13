using System;

namespace Astral.Schema.Generators
{
    public class InvalidServiceException : Exception
    {
        public InvalidServiceException()
        {
        }

        public InvalidServiceException(string message) : base(message)
        {
        }

        public InvalidServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}