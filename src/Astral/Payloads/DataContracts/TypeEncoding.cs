namespace Astral.Payloads.DataContracts
{
    public class TypeEncoding
    {
        public TypeEncoding(TypeToContract toContract, ContractToType toType)
        {
            ToContract = toContract;
            ToType = toType;
        }

        public TypeToContract ToContract { get; }
        public ContractToType ToType { get; }
        
        public static TypeEncoding Default(WellKnownTypes wellKnownTypes = null) =>
            new TypeEncoding(Contract.DefaultTypeMapper(wellKnownTypes ?? WellKnownTypes.Default).Loopback(), Contract.DefaultContractMapper(wellKnownTypes).Loopback());
            
    }
}