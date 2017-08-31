namespace Astral.Core
{
    public class ContractNameResolutionException : DataContractResolutionException
    {
        public ContractNameResolutionException(string contractName)
            : base($"Cannot determine type for contract {contractName}")

        {
        }
    }
}