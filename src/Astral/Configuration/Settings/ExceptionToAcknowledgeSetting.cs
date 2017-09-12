using System;

namespace Astral.Configuration.Settings
{
    
    public sealed class ExceptionToAcknowledgeSetting : Fact<Func<Exception, Acknowledge>>
    {
        public ExceptionToAcknowledgeSetting(Func<Exception, Acknowledge> value) : base(value)
        {
        }
    }
}