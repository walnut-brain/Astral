using System;
using System.Collections.Immutable;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public class WellKnownTypeDeclaration : ITypeDeclarationSchema
    {
        private readonly WellKnownTypeDescriptionSchemaGreen _green;

        public WellKnownTypeDeclaration(WellKnownTypeDescriptionSchemaGreen green)
        {
            _green = green;
        }

        public string Name => _green.Name;

        public string CodeName => _green.DotNetType.FullName;

        public string Code => _green.Name;

        public Type DotNetType => _green.DotNetType;

        public bool IsWellKnown => true;
    }
}