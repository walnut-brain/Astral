using System;
using LanguageExt;

namespace Astral.Payloads.Serialization
{
    public delegate Try<object> Deserialize<in T>(Type toType, T data);
}