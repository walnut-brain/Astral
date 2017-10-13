using System;
using System.Collections.Immutable;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public class WellKnownTypeSchema : ITypeDeclarationSchema
    {
        private readonly WellKnownTypeSchemaGreen _green;

        public WellKnownTypeSchema(WellKnownTypeSchemaGreen green)
        {
            _green = green;
        }

        public string SchemaName => _green.SchemaName;

        public string CodeName => _green.SchemaName;

        public string ContractName => _green.SchemaName;

        public Type DotNetType => _green.DotNetType.IfNoneDefault();

        public bool IsWellKnown => true;
    }
}