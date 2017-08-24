using System;
using LanguageExt;

namespace Astral.DataContracts
{
    public interface ITypeToContractName
    {
        Try<string> Map(Type type, object data);
    }
}