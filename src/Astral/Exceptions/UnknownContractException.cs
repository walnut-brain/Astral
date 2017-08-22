using System;

namespace Astral.Exceptions
{
    public class UnknownContractException : Exception
    {
        public UnknownContractException()
        {
        }

        public UnknownContractException(string message) : base(message)
        {
        }

        public UnknownContractException(string message, Exception innerException) : base(message, innerException)
        {
        }


    }
}