using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class ComplexTypeDesc : TypeDesc
    {
        public ComplexTypeDesc(Dictionary<string, TypeDesc> fields)
        {
            Fields = fields;
        }

        public ComplexTypeDesc(ComplexTypeDesc parent, IEnumerable<KeyValuePair<string, TypeDesc>> fields)
        {
            Parent = parent;
            Fields = new ReadOnlyDictionary<string, TypeDesc>(fields.ToDictionary(p => p.Key, p => p.Value));
        }

        public ComplexTypeDesc Parent { get; }
        public IReadOnlyDictionary<string, TypeDesc> Fields { get; }
    }
}