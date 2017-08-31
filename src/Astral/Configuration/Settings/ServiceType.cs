using System;
using Astral.Markup;
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