using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class MessageType : NewType<MessageType, Type>
    {
        public MessageType(Type value) : base(value)
        {
        }
    }
}