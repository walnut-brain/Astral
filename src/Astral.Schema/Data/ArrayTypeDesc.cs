namespace Astral.Schema.Data
{
    public sealed class ArrayTypeDesc : TypeDesc
    {
        public ArrayTypeDesc(TypeDesc elementType)
        {
            ElementType = elementType;
        }

        public TypeDesc ElementType { get; }
    }
}