using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class EnumTypeDesc : NamedTypeDesc
    {
        public EnumTypeDesc(string name, string contract, Type dotNetType, SimpleTypeDesc baseType, IEnumerable<KeyValuePair<string, long>> values)
            : base(name)
        {
            Contract = contract;
            DotNetType = dotNetType;
            BaseType = baseType;
            Values = new ReadOnlyDictionary<string, long>(values.ToDictionary(p => p.Key, p => p.Value));
        }

        public SimpleTypeDesc BaseType { get; }
        public IReadOnlyDictionary<string, long> Values { get; }

        public override string Contract { get; }
        public override Type DotNetType { get; }
    }
}