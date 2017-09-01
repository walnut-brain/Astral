using System;
using LanguageExt;

namespace Astral.Payloads.Contracts
{
    public delegate Try<string> ComplexTypeToContract(Type type, TypeToContract elementResolver);
}