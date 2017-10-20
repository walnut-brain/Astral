using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IServiceSchema
    {
        string Name { get; }
        string Owner { get; }
        string CodeName { get; }
        IReadOnlyCollection<IEventSchema> Events { get; }
        IReadOnlyCollection<ICallSchema> Calls { get; }
        IReadOnlyCollection<ITypeSchema> Types { get; }
        IEventSchema EventByCodeName(string codeName);
        ICallSchema CallByCodeName(string codeName);

    }
}