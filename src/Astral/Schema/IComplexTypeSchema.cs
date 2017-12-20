using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IComplexTypeSchema : ITypeSchema
    {
        bool IsStruct { get; }
        IComplexTypeSchema BaseOn { get;  }
        IReadOnlyDictionary<string, ITypeSchema> Fields { get;  }
    }
}