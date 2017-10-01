using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class EnumTypeDesc : TypeDesc
    {
        public EnumTypeDesc(SimleTypeDesc baseType, IEnumerable<KeyValuePair<string, long>> values)
        {
            BaseType = baseType;
            Values = new ReadOnlyDictionary<string, long>(values.ToDictionary(p => p.Key, p => p.Value));
        }

        public SimleTypeDesc BaseType { get; }
        public IReadOnlyDictionary<string, long> Values { get; }
    }
}