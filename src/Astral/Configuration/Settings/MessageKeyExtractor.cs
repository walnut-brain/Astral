using System;

namespace Astral.Configuration.Settings
{
    public class MessageKeyExtractor<TMessage> : Fact<Func<TMessage, string>>
    {
        public MessageKeyExtractor(Func<TMessage, string> value) : base(value)
        {
        }
    }
}