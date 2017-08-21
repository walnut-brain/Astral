using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ServiceType : NewType<ServiceType, Type>
    {
        public ServiceType(Type value) : base(value)
        {
        }
    }
}