using System;

namespace Astral.Payloads
{
    public class PayloadException : Exception
    {
        public PayloadException()
        {
        }

        public PayloadException(string message) : base(message)
        {
        }

        public PayloadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}