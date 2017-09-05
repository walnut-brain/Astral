using System.Collections.Immutable;
using System.Net.Mime;

namespace Astral.Payloads.Serialization
{
    public delegate ImmutableList<Serialize<T>> SerializeProvider<T>(ContentType contentType);
}