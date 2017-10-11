using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class EnumTypeDesc : NamedTypeDesc
    {
        public EnumTypeDesc(Type dotNetType, string name, string contract, SimpleTypeDesc baseType, IEnumerable<KeyValuePair<string, long>> values)
            : base(dotNetType, name, contract)
        {

            BaseType = baseType;
            Values = new ReadOnlyDictionary<string, long>(values.ToDictionary(p => p.Key, p => p.Value));
        }

        public SimpleTypeDesc BaseType { get; }
        public IReadOnlyDictionary<string, long> Values { get; }

    }
}