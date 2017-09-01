using System;
using Astral.Payloads;
using LanguageExt;

namespace Astral.Serialization
{
    public interface IDeserialize<TFormat>
    {
        Try<object> Deserialize(Type type, PayloadBase<TFormat> data);
    }
}