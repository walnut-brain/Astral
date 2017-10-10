namespace Astral.Schema
{
    public interface IEndpointSchema : ISchema
    {
        IServiceSchema Service { get; }
        string Name { get; }
        string CodeName();
    }
}