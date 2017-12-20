using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IEnumTypeSchema : ITypeSchema
    {
        ITypeSchema BaseOn { get;  }
        IReadOnlyDictionary<string, long> Values { get; }
        bool IsFlags { get; }
    }
}