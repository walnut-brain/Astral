using System.Net.Mime;
using LanguageExt;

namespace Astral.Payloads.Serialization
{
    public delegate Try<(ContentType, T)> Serialize<T>(object value);
}