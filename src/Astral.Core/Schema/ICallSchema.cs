using System;

namespace Astral.Schema
{
    public interface ICallSchema : IEndpointSchema
    {
        ITypeSchema RequestType { get; }
        ITypeSchema ResponseType { get; }
    }
}