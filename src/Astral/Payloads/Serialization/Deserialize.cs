using System;
using FunEx;
using FunEx.Monads;

namespace Astral.Payloads.Serialization
{
    public delegate Result<object> Deserialize<in T>(Type toType, T data);
}