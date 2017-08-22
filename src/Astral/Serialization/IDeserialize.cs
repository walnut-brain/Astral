using System;
using LanguageExt;

namespace Astral.Serialization
{
    public interface IDeserialize<TFormat>
    {
        Try<object> Deserialize(Type type, Serialized<TFormat> data);
    }
}