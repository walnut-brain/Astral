using System;

namespace Astral.Configuration.Settings
{
    public sealed class MessageTtl : Fact<TimeSpan>
    {
        public MessageTtl(TimeSpan value) : base(value)
        {
        }
    }
}