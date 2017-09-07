using System;

namespace Astral.Configuration.Settings
{
    
    public sealed class RecieveExceptionPolicy : Fact<Func<Exception, Acknowledge>>
    {
        public RecieveExceptionPolicy(Func<Exception, Acknowledge> value) : base(value)
        {
        }
    }
}