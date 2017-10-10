using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IComplexServiceSchema : IServiceSchema
    {
        IEnumerable<IEventSchema> Events { get; }
        IEnumerable<ICallSchema> Calls { get; }
    }
}