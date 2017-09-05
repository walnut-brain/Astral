using System;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceType : Fact<Type>
    {
        public ServiceType(Type value) : base(value)
        {
        }
    }
}