namespace Astral.Payloads.DataContracts
{
    public class TypeDecoderException : TypeEncodingException
    {
        public TypeDecoderException(string contract)
            : base($"Cannot determine type for contract {contract}")
        {
            Contract = contract;
        }

        public TypeDecoderException(string contract, string message)
            : base(message)
        {
            Contract = contract;
        }

        public string Contract { get; }
    }
}