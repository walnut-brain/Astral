using System;
using CsFun;

namespace Astral.Payloads.DataContracts
{
    public delegate Result<string> ComplexTypeToContract(Type type, TypeToContract elementResolver);
}