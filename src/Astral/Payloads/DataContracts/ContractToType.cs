using System;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public delegate Try<Type> ContractToType(string contract, Seq<Type> awaited);
}