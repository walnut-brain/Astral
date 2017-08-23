using System;

namespace Astral.Exceptions
{
    public class RequeuException : AcknowledgeException
    {
        public RequeuException() : base(Acknowledge.Requeue)
        {
        }

        public RequeuException(string message) : base(Acknowledge.Requeue, message)
        {
        }
    }
}