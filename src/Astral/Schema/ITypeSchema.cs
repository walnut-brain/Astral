using System;

namespace Astral.Schema
{
    public interface ITypeSchema 
    {
        string ContractName { get; }
        string CodeName { get; }
        string SchemaName { get; }
        Type DotNetType { get; }
        bool IsWellKnown { get; }
    }
}