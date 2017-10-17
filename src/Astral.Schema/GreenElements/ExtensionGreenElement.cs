using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Astral.Schema
{
    internal class ExtensionGreenElement : SchemaGreenElement
    {
        private ExtensionGreenElement(long id, string name, ValueGreenElement value) : base(id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if(IsReserved(name))
                throw new ArgumentOutOfRangeException(nameof(name), $"Word {name} is reserved!");
            Name = name;
            Value = value ?? NullValueGreenElement.Value;
        }

        public ExtensionGreenElement(string name, ValueGreenElement value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if(IsReserved(name))
                throw new ArgumentOutOfRangeException(nameof(name), $"Word {name} is reserved!");
            Name = name;
            Value = value ?? NullValueGreenElement.Value;
        }

        public string Name { get; }
        public ValueGreenElement Value { get; }

        protected override IReadOnlyCollection<SchemaGreenElement> Children { get; } =
            ImmutableList<SchemaGreenElement>.Empty;

        
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public ExtensionGreenElement With(OptionalParameter<string> Name = default,
            OptionalParameter<ValueGreenElement> Value = default)
        {
            var name = Name.WhenSkipped(this.Name);
            var value = Value.WhenSkipped(this.Value) ?? NullValueGreenElement.Value;
            if(name != this.Name || value != this.Value)
                return new ExtensionGreenElement(Id, name, value);
            return this;
        }

        protected bool Equals(ExtensionGreenElement other)
        {
            return base.Equals(other) && StringComparer.InvariantCultureIgnoreCase.Equals(Name, other.Name) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ExtensionGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}