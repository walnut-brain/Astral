using System;

namespace Astral.Configuration.Settings
{
    public sealed class RpcTimeout : Fact<TimeSpan> 
    {
        public RpcTimeout(TimeSpan value) : base(value)
        {
            if(value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}