using System.Collections.Immutable;
using System.Net.Mime;
using FunEx;


namespace Astral.Payloads.Serialization
{
    public delegate ImmutableList<Deserialize<T>> DeserializeProvider<T>(Option<ContentType> contentType);
}