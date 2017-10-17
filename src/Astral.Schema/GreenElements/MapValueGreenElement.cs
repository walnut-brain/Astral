using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Astral.Schema
{
    internal class MapValueGreenElement : ValueGreenElement, IReadOnlyDictionary<string, ValueGreenElement>
    {
        private readonly ImmutableDictionary<string, ValueGreenElement> _elements;
        private readonly Lazy<IReadOnlyCollection<ValueGreenElement>> _children;

        private MapValueGreenElement(long id, IEnumerable<KeyValuePair<string, ValueGreenElement>> items = null) :
            base(id)
        {
            _elements = items == null 
                ? ImmutableDictionary<string, ValueGreenElement>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase) 
                : items is ImmutableDictionary<string, ValueGreenElement> d 
                    ? d 
                    : ImmutableDictionary.CreateRange(StringComparer.InvariantCultureIgnoreCase, items);
            _children = new Lazy<IReadOnlyCollection<ValueGreenElement>>(() => ImmutableList.CreateRange(_elements.Values));
        }
        
        public MapValueGreenElement(params (string, ValueGreenElement)[] items) :
            this(items.Select(p => new KeyValuePair<string, ValueGreenElement>(p.Item1, p.Item2)))
        {
        }

        public MapValueGreenElement(IEnumerable<KeyValuePair<string, ValueGreenElement>> items = null)
        {
            _elements = items == null 
                ? ImmutableDictionary<string, ValueGreenElement>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase) 
                : items is ImmutableDictionary<string, ValueGreenElement> d 
                    ? d 
                    : ImmutableDictionary.CreateRange(StringComparer.InvariantCultureIgnoreCase, items);
            _children = new Lazy<IReadOnlyCollection<ValueGreenElement>>(() => ImmutableList.CreateRange(_elements.Values));
        }


        protected override IReadOnlyCollection<SchemaGreenElement> Children  => _children.Value;

        public ValueGreenElement this[string property] => _elements[property];

        public bool TryGetValue(string property, out ValueGreenElement value)
            => _elements.TryGetValue(property, out value);

        private MapValueGreenElement Change(
            Func<ImmutableDictionary<string, ValueGreenElement>, ImmutableDictionary<string, ValueGreenElement>> change)
        {
            var newMap = change(_elements);
            return newMap == _elements ? this : new MapValueGreenElement(Id, newMap);
        }

        public MapValueGreenElement SetValue(string property, ValueGreenElement value)
            => Change(p => p.SetItem(property, value));

        public MapValueGreenElement SetValues(IEnumerable<KeyValuePair<string, ValueGreenElement>> values)
            => Change(p => p.SetItems(values));
        
        public MapValueGreenElement SetValues(IEnumerable<(string, ValueGreenElement)> values)
            => Change(p => p.SetItems(values.Select(v => new KeyValuePair<string, ValueGreenElement>(v.Item1, v.Item2))));
        
        public MapValueGreenElement SetValues(params (string, ValueGreenElement)[] values)
            => Change(p => p.SetItems(values.Select(v => new KeyValuePair<string, ValueGreenElement>(v.Item1, v.Item2))));

        public MapValueGreenElement Add(string property, ValueGreenElement value)
            => Change(p => p.Add(property, value));
        
        public MapValueGreenElement AddRange(IEnumerable<KeyValuePair<string, ValueGreenElement>> values)
            => Change(p => p.AddRange(values));
        
        public MapValueGreenElement AddRange(IEnumerable<(string, ValueGreenElement)> values)
            => Change(p => p.AddRange(values.Select(v => new KeyValuePair<string, ValueGreenElement>(v.Item1, v.Item2))));
        
        public MapValueGreenElement AddRange(params (string, ValueGreenElement)[] values)
            => Change(p => p.AddRange(values.Select(v => new KeyValuePair<string, ValueGreenElement>(v.Item1, v.Item2))));

        public MapValueGreenElement Remove(string property)
            => Change(p => p.Remove(property));

        public MapValueGreenElement RemoveRange(IEnumerable<string> properties)
            => Change(p => p.RemoveRange(properties));
        
        public MapValueGreenElement RemoveRange(params string[] properties)
            => Change(p => p.RemoveRange(properties));


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, ValueGreenElement>> GetEnumerator()
            => _elements.GetEnumerator();

        public int Count => _elements.Count;


        public bool ContainsKey(string key) => _elements.ContainsKey(key);


        public IEnumerable<string> Keys => _elements.Keys;


        public IEnumerable<ValueGreenElement> Values => _elements.Values;

        protected bool Equals(MapValueGreenElement other)
        {
            return base.Equals(other) && _elements.Equals(other._elements);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MapValueGreenElement) obj);
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