namespace Astral.Exceptions
{
    public class ContractNameResolutionException : DataContractResolutionException
    {
        public ContractNameResolutionException(string contractName)
            : base($"Cannot determine type for contract {contractName}")

        {
        }
    }
}