using System.Net.Mime;
using FunEx;
using FunEx.Monads;

namespace Astral.Payloads.Serialization
{
    public delegate Result<(ContentType, T)> Serialize<T>(object value);
}