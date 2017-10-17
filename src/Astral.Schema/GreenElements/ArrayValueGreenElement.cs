using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Astral.Schema
{
    [SuppressMessage("ReSharper", "ImpureMethodCallOnReadonlyValueField")]
    internal class ArrayValueGreenElement : ValueGreenElement, IReadOnlyList<ValueGreenElement>
    {
        private readonly ImmutableArray<ValueGreenElement> _values;

        public ArrayValueGreenElement(IEnumerable<ValueGreenElement> values)
        {
            _values = values is ImmutableArray<ValueGreenElement> a ? a : ImmutableArray.CreateRange(values);
        }

        public ArrayValueGreenElement(params ValueGreenElement[] values)
            : this((IEnumerable<ValueGreenElement>) values)
        {
            
        }

        private ArrayValueGreenElement(long id, ImmutableArray<ValueGreenElement> values) : base(id)
        {
            _values = values;
        }

        protected override IReadOnlyCollection<SchemaGreenElement> Children => _values;

        public ValueGreenElement this[int index] => _values[index];

        private ArrayValueGreenElement Change(
            Func<ImmutableArray<ValueGreenElement>, ImmutableArray<ValueGreenElement>> changer)
        {
            var newItems = changer(_values);
            if (newItems == _values) return this;
            return new ArrayValueGreenElement(Id, newItems);
        }
        
        public ArrayValueGreenElement SetItem(int index, ValueGreenElement value)
            => Change(p => p.SetItem(index, value));
            

        public ArrayValueGreenElement AddItem(ValueGreenElement value)
            => Change(p => p.Add(value));
        
        public ArrayValueGreenElement InsertItem(int index, ValueGreenElement value)
            => Change(p => p.Insert(index, value));
        
        public ArrayValueGreenElement AddRange(IEnumerable<ValueGreenElement> items)
            => Change(p => p.AddRange(items));
        
        public ArrayValueGreenElement AddRange(params ValueGreenElement[] items)
            => Change(p => p.AddRange(items));
        
        public ArrayValueGreenElement InsertRange(int index, IEnumerable<ValueGreenElement> items)
            => Change(p => p.InsertRange(index, items));
        
        public ArrayValueGreenElement InsertRange(int index, params ValueGreenElement[] items)
            => Change(p => p.InsertRange(index, items));

        public ArrayValueGreenElement Remove(ValueGreenElement item)
            => Change(p => p.Remove(item));

        public ArrayValueGreenElement RemoveAt(int index)
            => Change(p => p.RemoveAt(index));
        
        public ArrayValueGreenElement RemoveRange(IEnumerable<ValueGreenElement> items)
            => Change(p => p.RemoveRange(items));

        public ArrayValueGreenElement RemoveRange(params ValueGreenElement[] items)
            => Change(p => p.RemoveRange(items));

        public ArrayValueGreenElement RemoveRange(int index, int length)
            => Change(p => p.RemoveRange(index, length));
        
        public ArrayValueGreenElement Replace(ValueGreenElement oldValue, ValueGreenElement newValue)
            => Change(p => p.Replace(oldValue, newValue));

        public int IndexOf(ValueGreenElement item)
            => _values.IndexOf(item);
        
        public int IndexOf(ValueGreenElement item, int startIndex)
            => _values.IndexOf(item, startIndex);
        
        public int LastIndexOf(ValueGreenElement item)
            => _values.LastIndexOf(item);
        
        public int LastIndexOf(ValueGreenElement item, int startIndex)
            => _values.LastIndexOf(item, startIndex);

        IEnumerator  IEnumerable.GetEnumerator() => ((IEnumerable) _values).GetEnumerator();


        public ArrayValueGreenElement Clear() => new ArrayValueGreenElement(Id, ImmutableArray<ValueGreenElement>.Empty);


        public int IndexOf(ValueGreenElement item, int startIndex, int count) 
            => _values.IndexOf(item, startIndex, count);

        public int LastIndexOf(ValueGreenElement item, int startIndex, int count) 
            => _values.LastIndexOf(item, startIndex, count);

        
        public bool Contains(ValueGreenElement item) => _values.Contains(item);

        
        public ImmutableArray<ValueGreenElement> RemoveAll(Predicate<ValueGreenElement> match) 
            => _values.RemoveAll(match);


        IEnumerator<ValueGreenElement> IEnumerable<ValueGreenElement>.GetEnumerator()
        {
            return ((IEnumerable<ValueGreenElement>) _values).GetEnumerator();
        }

        public bool IsEmpty => _values.IsEmpty;

        public int Length => _values.Length;

        public int Count => Length;

        protected bool Equals(ArrayValueGreenElement other)
        {
            return base.Equals(other) && _values.Equals(other._values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ArrayValueGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _values.GetHashCode();
            }
        }
    }
}