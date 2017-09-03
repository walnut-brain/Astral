using System;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class ResponseType : Fact<Type>
    {
        public ResponseType(Type value) : base(value)
        {
        }
    }
}