using System;
using FunEx;
using FunEx.Monads;

namespace Astral.Payloads.DataContracts
{
    public delegate Result<string> TypeToContract(Type type);
}