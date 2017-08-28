using System;

namespace Astral.Exceptions
{
    public class CannotTakeLeaseException : Exception
    {
        public CannotTakeLeaseException()
        {
        }

        public CannotTakeLeaseException(string message) : base(message)
        {
        }

        public CannotTakeLeaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}