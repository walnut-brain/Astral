using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class MessageTtl : NewType<MessageTtl, TimeSpan>
    {
        public MessageTtl(TimeSpan value) : base(value)
        {
        }
    }
}