using System;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class ServiceType : Fact<Type>
    {
        public ServiceType(Type value) : base(value)
        {
        }
    }
}