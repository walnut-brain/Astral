namespace Astral.Schema
{
    public interface IArrayTypeSchema : ITypeSchema
    {
        ITypeSchema ElementType { get; }
    }
}