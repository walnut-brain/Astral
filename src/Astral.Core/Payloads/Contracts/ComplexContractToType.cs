using System;
using LanguageExt;

namespace Astral.Payloads.Contracts
{
    public delegate Try<Type> ComplexContractToType(string contract, Seq<Type> awaited, ContractToType elementResolver);
}