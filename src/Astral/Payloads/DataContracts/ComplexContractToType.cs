using System;
using System.Collections.Immutable;
using CsFun;

namespace Astral.Payloads.DataContracts
{
    public delegate Result<Type> ComplexContractToType(string contract, ImmutableList<Type> awaited, ContractToType elementResolver);
}