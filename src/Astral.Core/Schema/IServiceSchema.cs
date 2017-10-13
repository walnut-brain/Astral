using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    public interface IServiceSchema
    {
        string Name { get; }
        string Owner { get; }
        string CodeName { get; }
        IEnumerable<IEventSchema> Events { get; }
        IEnumerable<ICallSchema> Calls { get; }
        IEnumerable<ITypeDeclarationSchema> Types { get; }
        IEventSchema EventByCodeName(string codeName);
        ICallSchema CallByCodeName(string codeName);

    }
}