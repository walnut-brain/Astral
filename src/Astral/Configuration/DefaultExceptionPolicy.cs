using System;
using System.Linq;
using System.Threading.Tasks;
using Astral.Exceptions;

namespace Astral.Configuration
{
    public class DefaultExceptionPolicy : IReciveExceptionPolicy
    {
        public Acknowledge WhenException(Exception exception)
        {
            if(exception is RequeuException) return Acknowledge.Requeue;
            if(exception is TaskCanceledException) return Acknowledge.Requeue;
            if(exception is AggregateException ae && ae.Flatten().InnerExceptions.Any(p => p is TaskCanceledException))
                return Acknowledge.Requeue;
            return Acknowledge.Nack;
        }
    }
}