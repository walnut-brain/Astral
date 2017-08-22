using System;
using LanguageExt;

namespace Astral.DataContracts
{
    public interface IContractNameToType
    {
        Result<Type> TryMap(string contractName, Type awaited);
    }
}