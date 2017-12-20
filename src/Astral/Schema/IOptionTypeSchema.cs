namespace Astral.Schema
{
    public interface IOptionTypeSchema : ITypeSchema
    {
        ITypeSchema ElementType { get;  }
    }
}