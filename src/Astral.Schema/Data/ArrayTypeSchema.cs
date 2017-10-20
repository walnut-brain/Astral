using System;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public sealed class ArrayTypeSchema : IArrayTypeSchema
    {
        public ServiceSchema Service { get; }
        private ArrayTypeSchemaGreen Green { get; }

        public ArrayTypeSchema(ServiceSchema service, ArrayTypeSchemaGreen green)
        {
            Service = service;
            Green = green;
        }

        public string SchemaName => ElementType.SchemaName != null ? ElementType.SchemaName + "[]" : null;


        public string CodeName => ElementType.CodeName != null ? ElementType.CodeName + "[]" : null;


        public string ContractName => ElementType.ContractName != null ? ElementType.ContractName + "[]" : null;


        public Type DotNetType => Green.DotNetType.IfNoneDefault();


        public bool IsWellKnown => true;


        public ITypeSchema ElementType => Service.TypeById(Green.ElementId);

    }
}