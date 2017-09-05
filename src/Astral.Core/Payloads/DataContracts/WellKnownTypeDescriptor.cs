using System;

namespace Astral.Payloads.DataContracts
{
    public class WellKnownTypeDescriptor
    {
        public WellKnownTypeDescriptor(Type type, string code, string description)
        {
            Type = type;
            Code = code;
            Description = description;
        }

        public Type Type { get; }
        public string Code { get; }
        public string Description { get; }
    }
}