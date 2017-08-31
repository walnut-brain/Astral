using System;
using LanguageExt;

namespace Astral.Core
{
    public interface IContractToType
    {
        Option<Type> TryMap(string contractName, Seq<Type> awaited);
    }
}