using System;

namespace Astral.Exceptions
{
    public class AcknowledgeException : Exception
    {
        public Acknowledge Acknowledge { get; }

        public AcknowledgeException(Acknowledge acknowledge)
        {
            Acknowledge = acknowledge;
        }

        public AcknowledgeException(Acknowledge acknowledge, string message) : base(message)
        {
            Acknowledge = acknowledge;
        }
    }
}