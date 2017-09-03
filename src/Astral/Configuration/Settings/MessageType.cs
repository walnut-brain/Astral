using System;
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