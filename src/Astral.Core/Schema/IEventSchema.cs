using System;

namespace Astral.Schema
{
    public interface IEventSchema : IEndpointSchema
    {
        ITypeDeclarationSchema EventType { get; }
    }
}