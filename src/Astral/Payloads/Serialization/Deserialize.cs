using System;
using FunEx;

namespace Astral.Payloads.Serialization
{
    public delegate Result<object> Deserialize<in T>(Type toType, T data);
}