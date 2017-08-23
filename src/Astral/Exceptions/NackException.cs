using System;

namespace Astral.Exceptions
{
    public class NackException : AcknowledgeException
    {
        public NackException() : base(Acknowledge.Nack)
        {
        }

        public NackException(string message) : base(Acknowledge.Nack, message)
        {
        }
    }
}