using System;
using LanguageExt;

namespace Astral.Payloads.Contracts
{
    public delegate Try<string> TypeToContract(Type type);
}