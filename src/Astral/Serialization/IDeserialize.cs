using System;
using Astral.Core;
using LanguageExt;

namespace Astral.Serialization
{
    public interface IDeserialize<TFormat>
    {
        Try<object> Deserialize(Type type, Payload<TFormat> data);
    }
}