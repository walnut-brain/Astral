namespace Astral.Schema.Data
{
    public sealed class SimleTypeDesc : TypeDesc
    {
        public SimleTypeDesc(SimleTypeKind kind)
        {
            Kind = kind;
        }

        public SimleTypeKind Kind { get; }
    }
}