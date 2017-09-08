using System.Collections.Immutable;
using System.Net.Mime;
using FunEx;
using FunEx.Monads;


namespace Astral.Payloads.Serialization
{
    public delegate ImmutableList<Deserialize<T>> DeserializeProvider<T>(Option<ContentType> contentType);
}