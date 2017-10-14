using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IEnumTypeSchema : ITypeSchema
    {
        ITypeSchema BaseOn { get;  }
        bool IsFlags { get; }
        IReadOnlyDictionary<string, long> Values { get; }
    }
}