using System;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public class NullableTypeSchema : IOptionTypeDeclarationSchema
    {
        public ServiceSchema Service { get; }
        private readonly NullableTypeSchemaGreen _green;

        public NullableTypeSchema(ServiceSchema service, NullableTypeSchemaGreen green)
        {
            Service = service;
            _green = green;
        }

        public string ContractName => ElementType.ContractName != null ? ElementType.ContractName + "?" : null;


        public string CodeName => ElementType.CodeName != null ? ElementType.CodeName + "?" : null;


        public string SchemaName => ElementType.SchemaName != null ? ElementType.SchemaName + "?" : null;


        public Type DotNetType => _green.DotNetType.IfNoneDefault();


        public bool IsWellKnown => true;

        public ITypeDeclarationSchema ElementType => Service.TypeById(_green.ElementTypeId);

    }
}