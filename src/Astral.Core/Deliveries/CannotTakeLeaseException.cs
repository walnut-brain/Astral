using System;

namespace Astral.Deliveries
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