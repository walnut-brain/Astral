using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class ComplexTypeDesc : NamedTypeDesc
    {
        public ComplexTypeDesc(string name, string contract, Type dotNetType, Dictionary<string, TypeDesc> fields) : base(name)
        {
            Contract = contract;
            DotNetType = dotNetType;
            Fields = new ReadOnlyDictionary<string, TypeDesc>(fields);
        }

        public ComplexTypeDesc(string name, string contract, Type dotNetType, ComplexTypeDesc parent, IEnumerable<KeyValuePair<string, TypeDesc>> fields)
            : base(name)
        {
            Contract = contract;
            DotNetType = dotNetType;
            Parent = parent;
            Fields = new ReadOnlyDictionary<string, TypeDesc>(fields.ToDictionary(p => p.Key, p => p.Value));
        }

        public ComplexTypeDesc Parent { get; }
        public IReadOnlyDictionary<string, TypeDesc> Fields { get; }

        public override string Contract { get; }

        public override Type DotNetType { get; }
    }
}