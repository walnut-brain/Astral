using System;
using Astral.Markup;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class MessageType : Fact<Type>
    {
        public MessageType(Type value) : base(value)
        {
        }
    }
}