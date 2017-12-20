using System.Net.Mime;

namespace Astral.Schema
{
    public interface IEndpointSchema 
    {
        IServiceSchema Service { get; }
        string Name { get; }
        string CodeName { get; }
        ContentType ContentType { get; }
    }
}