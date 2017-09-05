namespace Astral.Payloads.DataContracts
{
    public class ContractToTypeException : ContractResolutionException
    {
        public ContractToTypeException(string contract)
            : base($"Cannot determine type for contract {contract}")
        {
            Contract = contract;
        }

        public ContractToTypeException(string contract, string message)
            : base(message)
        {
            Contract = contract;
        }

        public string Contract { get; }
    }
}