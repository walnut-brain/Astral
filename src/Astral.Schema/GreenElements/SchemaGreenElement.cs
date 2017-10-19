using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Astral.Schema
{
    internal abstract class SchemaGreenElement : Node
    {
        protected SchemaGreenElement()
        {
        }

        protected SchemaGreenElement(SchemaGreenElement @base) : base(@base)
        {
        }

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