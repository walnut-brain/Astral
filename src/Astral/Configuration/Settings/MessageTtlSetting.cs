using System;

namespace Astral.Configuration.Settings
{
    public sealed class MessageTtlSetting : Fact<TimeSpan>
    {
        public MessageTtlSetting(TimeSpan value) : base(value)
        {
        }
    }
}