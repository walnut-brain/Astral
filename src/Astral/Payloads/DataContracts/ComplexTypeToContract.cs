using System;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public delegate Try<string> ComplexTypeToContract(Type type, TypeToContract elementResolver);
}