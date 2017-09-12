using System;

namespace Astral.Configuration.Settings
{
    public sealed class MessageTtlFactorySetting<T> : Fact<Func<T, TimeSpan>>
    {
        public MessageTtlFactorySetting(Func<T, TimeSpan> value) : base(value)
        {
        }
    }
}