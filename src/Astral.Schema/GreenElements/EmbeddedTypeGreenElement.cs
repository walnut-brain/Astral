using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    internal class EmbeddedTypeGreenElement : TypeOrTypeReferenceElement
    {
        public EmbeddedTypeGreenElement(long id, TypeGreenElement type) : base(id)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public EmbeddedTypeGreenElement(TypeGreenElement type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public TypeGreenElement Type { get; }

        public EmbeddedTypeGreenElement With(TypeGreenElement type)
        {
            if (Equals(type, Type))
                return this;
            return new EmbeddedTypeGreenElement(Id, type);
        }

        protected override IReadOnlyCollection<SchemaGreenElement> Children => new[] {Type};

        protected bool Equals(EmbeddedTypeGreenElement other)
        {
            return base.Equals(other) && Type.Equals(other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EmbeddedTypeGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Type.GetHashCode();
            }
        }
    }
}