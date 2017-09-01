using System.Net.Mime;
using LanguageExt;

namespace Astral.Payloads.Serialization
{
    public delegate Seq<Deserialize<T>> DeserializeProvider<T>(Option<ContentType> contentType);
}