using System;

namespace Astral.DataContracts
{
    public interface ITypeToContractName
    {
        string Map(Type type, object data);
    }
}