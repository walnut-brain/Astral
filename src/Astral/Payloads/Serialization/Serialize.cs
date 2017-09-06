using System.Net.Mime;
using FunEx;

namespace Astral.Payloads.Serialization
{
    public delegate Result<(ContentType, T)> Serialize<T>(object value);
}