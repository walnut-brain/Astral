using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IComplexServiceSchema : IServiceSchema
    {
        IEnumerable<IEventSchema> Events { get; }
        IEnumerable<ICallSchema> Calls { get; }
        IEnumerable<ITypeSchema> Types { get; }
        IReadOnlyDictionary<string, IEventSchema> EventByCodeName { get; }
        IReadOnlyDictionary<string, ICallSchema> CallByCodeName { get; }
    }
}