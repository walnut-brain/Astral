using System.Collections.Immutable;
using System.Net.Mime;
using CsFun;


namespace Astral.Payloads.Serialization
{
    public delegate ImmutableList<Deserialize<T>> DeserializeProvider<T>(Option<ContentType> contentType);
}