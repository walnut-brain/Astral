using System;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceTypeSetting : Fact<Type>
    {
        public ServiceTypeSetting(Type value) : base(value)
        {
        }
    }
}