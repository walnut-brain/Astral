using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Astral.Schema
{
    internal class ExtensionCollectionGreenElement : SchemaGreenElement, IReadOnlyDictionary<string, ExtensionGreenElement>
    {
        private readonly ImmutableDictionary<string, ExtensionGreenElement> _elements;
        private readonly Lazy<IReadOnlyCollection<SchemaGreenElement>> _children;

        private ExtensionCollectionGreenElement(long id, ImmutableDictionary<string, ExtensionGreenElement> elements) : base(id)
        {
            _elements = elements;
            _children = new Lazy<IReadOnlyCollection<SchemaGreenElement>>(() => ImmutableList.CreateRange(_elements.Values));
        }

        public ExtensionCollectionGreenElement(IEnumerable<ExtensionGreenElement> elements)
        {
            _elements = elements == null 
                ? ImmutableDictionary<string, ExtensionGreenElement>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase) 
                : ImmutableDictionary.CreateRange(StringComparer.InvariantCultureIgnoreCase, elements.Select(p => new KeyValuePair<string, ExtensionGreenElement>(p.Name, p)));
        }

        public ExtensionGreenElement this[string name] => _elements[name];

        public bool TryGetValue(string name, out ExtensionGreenElement value)
            => _elements.TryGetValue(name, out value);

        private ExtensionCollectionGreenElement Change(
            Func<ImmutableDictionary<string, ExtensionGreenElement>, ImmutableDictionary<string, ExtensionGreenElement>>
                changer)
        {
            var newMap = changer(_elements);
            if(newMap != _elements)
                return new ExtensionCollectionGreenElement(Id, newMap);
            return this;
        }

        public ExtensionCollectionGreenElement SetItem(ExtensionGreenElement extension)
            => Change(p => p.SetItem(extension.Name, extension));
        
        public ExtensionCollectionGreenElement SetItems(IEnumerable<ExtensionGreenElement> extension)
            => Change(p => p.SetItems(extension.ToDictionary(t => t.Name)));
        
        public ExtensionCollectionGreenElement SetItems(params ExtensionGreenElement[] extension)
            => Change(p => p.SetItems(extension.ToDictionary(t => t.Name)));

        public ExtensionCollectionGreenElement AddItem(ExtensionGreenElement extension)
            => Change(p => p.Add(extension.Name, extension));
        
        public ExtensionCollectionGreenElement AddRange(IEnumerable<ExtensionGreenElement> extensions)
            => Change(p => p.AddRange(extensions.ToDictionary(t => t.Name)));

        public ExtensionCollectionGreenElement Remove(string name)
            => Change(p => p.Remove(name));

        public ExtensionCollectionGreenElement RemoveRange(IEnumerable<string> names)
            => Change(p => p.RemoveRange(names));


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, ExtensionGreenElement>> GetEnumerator()
            => _elements.GetEnumerator();

        public int Count => _elements.Count;

        public bool ContainsKey(string key) => _elements.ContainsKey(key);


        public IEnumerable<string> Keys => _elements.Keys;

        public IEnumerable<ExtensionGreenElement> Values => _elements.Values;

        protected override IReadOnlyCollection<SchemaGreenElement> Children => _children.Value;

        protected bool Equals(ExtensionCollectionGreenElement other)
        {
            return base.Equals(other) && _elements.Equals(other._elements);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ExtensionCollectionGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _elements.GetHashCode();
            }
        }
    }
}