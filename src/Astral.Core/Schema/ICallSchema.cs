using System;

namespace Astral.Schema
{
    public interface ICallSchema : IEndpointSchema
    {
        ITypeDeclarationSchema RequestType { get; }
        ITypeDeclarationSchema ResponseType { get; }
    }
}