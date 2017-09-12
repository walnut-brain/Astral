using System;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class MessageTypeSetting : Fact<Type>
    {
        public MessageTypeSetting(Type value) : base(value)
        {
        }
    }
}