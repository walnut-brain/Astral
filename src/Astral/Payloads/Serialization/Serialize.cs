using System.Net.Mime;
using CsFun;

namespace Astral.Payloads.Serialization
{
    public delegate Result<(ContentType, T)> Serialize<T>(object value);
}