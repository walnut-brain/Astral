using System;

namespace Astral.Exceptions
{
    public class AcknowledgeException : Exception
    {
        public AcknowledgeException(Acknowledge acknowledge)
        {
            Acknowledge = acknowledge;
        }

        public AcknowledgeException(Acknowledge acknowledge, string message) : base(message)
        {
            Acknowledge = acknowledge;
        }

        public Acknowledge Acknowledge { get; }
    }
}