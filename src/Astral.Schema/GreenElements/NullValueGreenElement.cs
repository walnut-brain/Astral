using System.Collections.Generic;
using System.Collections.Immutable;

namespace Astral.Schema
{
    internal class NullValueGreenElement : ValueGreenElement
    {
        private NullValueGreenElement()
        {
        }

        public static NullValueGreenElement Value { get; } = new NullValueGreenElement();

        protected override IReadOnlyCollection<SchemaGreenElement> Children { get; } =
            ImmutableList<SchemaGreenElement>.Empty;        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return typeof(NullValueGreenElement).GetHashCode();
        }
    }
}