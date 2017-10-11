using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class ComplexTypeDesc : NamedTypeDesc
    {
        public ComplexTypeDesc(Type dotNetType, string name, string contract,  Dictionary<string, TypeDesc> fields) : base(dotNetType, name, contract)
        {
            
        
            Fields = new ReadOnlyDictionary<string, TypeDesc>(fields);
        }

        public ComplexTypeDesc(Type dotNetType, string name, string contract, ComplexTypeDesc parent, IEnumerable<KeyValuePair<string, TypeDesc>> fields)
            : base(dotNetType, name, contract)
        {
            Parent = parent;
            Fields = new ReadOnlyDictionary<string, TypeDesc>(fields.ToDictionary(p => p.Key, p => p.Value));
        }

        public ComplexTypeDesc Parent { get; }
        public IReadOnlyDictionary<string, TypeDesc> Fields { get; }

        
    }
}