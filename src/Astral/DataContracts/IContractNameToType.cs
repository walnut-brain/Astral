using System;
using LanguageExt;

namespace Astral.DataContracts
{
    public interface IContractNameToType
    {
        Try<Type> TryMap(string contractName, Option<Type> awaited);
    }
}