using System;
using Astral.Exceptions;

namespace Astral.Configuration
{
    public class DefaultExceptionPolicy : IReciveExceptionPolicy
    {
        public Acknowledge WhenException(Exception exception)
        {
            switch (exception)
            {
                case AcknowledgeException acke:
                    return acke.Acknowledge;
                default:
                    if (exception.IsCancellation())
                        return Acknowledge.Requeue;
                    return Acknowledge.Nack;
            }
        }
    }
}