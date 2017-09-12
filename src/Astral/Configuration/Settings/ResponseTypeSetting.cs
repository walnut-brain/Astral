using System;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class ResponseTypeSetting : Fact<Type>
    {
        public ResponseTypeSetting(Type value) : base(value)
        {
        }
    }
}