using System;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public delegate Try<Type> ComplexContractToType(string contract, Seq<Type> awaited, ContractToType elementResolver);
}