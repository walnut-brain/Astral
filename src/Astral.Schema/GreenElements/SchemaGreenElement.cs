using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Astral.Schema
{
    internal abstract class SchemaGreenElement
    {
        protected SchemaGreenElement(long id)
        {
            Id = id;
        }

        protected SchemaGreenElement()
        {
            Id = Interlocked.Increment(ref _idGen);
        }

        internal long Id { get; }

        protected abstract IReadOnlyCollection<SchemaGreenElement> Children { get; }

        internal SchemaGreenElement FindById(long id)
        {
            if (Id == id) return this;
            var chield = Children.FirstOrDefault(p => p.Id == id);
            if (chield != null) return chield;
            foreach (var child in Children)
            {
                var found = child.FindById(id);
                if (found != null) return found;
            }
            return null;
        }

        protected bool Equals(SchemaGreenElement other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SchemaGreenElement) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private static long _idGen;

        public static ImmutableSortedSet<string> ReservedWords { get; } =
            ImmutableSortedSet.CreateRange(StringComparer.InvariantCultureIgnoreCase, new[]
            {
                nameof(EndpointGreenElement.Name),
                nameof(EndpointGreenElement.ContentType),
                nameof(EndpointGreenElement.CodeNameHint),
                nameof(EventGreenElement.EventType)
            });

        public static bool IsReserved(string str) =>
            ReservedWords.Contains(str);
    }
}