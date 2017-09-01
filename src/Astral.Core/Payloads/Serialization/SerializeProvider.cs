using System.Net.Mime;
using LanguageExt;

namespace Astral.Payloads.Serialization
{
    public delegate Seq<Serialize<T>> SerializeProvider<T>(ContentType contentType);
}