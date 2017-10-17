using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Astral.Schema
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    internal class TypeReferenceElement : TypeOrTypeReferenceElement
    {
        private TypeReferenceElement(long id, long referenceId) : base(id)
        {
            ReferenceId = referenceId;
        }

        public TypeReferenceElement(TypeGreenElement type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            ReferenceId = type.Id;
        }

        public long ReferenceId { get; }

        public TypeReferenceElement With(TypeGreenElement type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.Id == ReferenceId)
                return this;
            return new TypeReferenceElement(Id, type.Id);
        }

        protected bool Equals(TypeReferenceElement other)
        {
            return base.Equals(other) && ReferenceId == other.ReferenceId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeReferenceElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ ReferenceId.GetHashCode();
            }
        }

        protected override IReadOnlyCollection<SchemaGreenElement> Children { get; } =
            ImmutableList<SchemaGreenElement>.Empty;

    }
}