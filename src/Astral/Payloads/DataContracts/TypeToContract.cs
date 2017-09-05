using System;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public delegate Try<string> TypeToContract(Type type);
}