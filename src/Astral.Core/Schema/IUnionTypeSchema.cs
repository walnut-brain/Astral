using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IUnionTypeSchema : ITypeSchema
    {
        IReadOnlyDictionary<string, ITypeSchema> Variants { get; }
    }
}