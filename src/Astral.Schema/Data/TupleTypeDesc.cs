using System.Collections.Generic;
using System.Linq;

namespace Astral.Schema.Data
{
    public sealed class TupleTypeDesc : TypeDesc
    {
        public TupleTypeDesc(IEnumerable<TypeDesc> elements)
        {
            Elements = elements.ToArray();
        }

        public IReadOnlyList<TypeDesc> Elements { get; }
    }
}