using System;

namespace Astral.Schema
{
    public interface IEventSchema : IEndpointSchema
    {
        Type ContractType();
        string ContractName();
    }
}