using System;

namespace Astral.Schema
{
    public interface IEventSchema : IEndpointSchema
    {
        ITypeSchema EventType { get; }
    }
}