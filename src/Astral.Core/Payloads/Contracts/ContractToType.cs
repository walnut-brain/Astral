using System;
using LanguageExt;

namespace Astral.Payloads.Contracts
{
    public delegate Try<Type> ContractToType(string contract, Seq<Type> awaited);
}