using System;
using System.Globalization;

namespace Astral.Schema
{
    public interface IServiceSchema : ISchema
    {
        string Name { get; }
        string Owner { get; }
        string CodeName();
        Type ServiceType();
    }
}