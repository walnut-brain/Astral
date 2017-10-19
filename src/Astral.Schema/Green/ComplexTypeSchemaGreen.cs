using System;
using System.Collections.Immutable;

namespace Astral.Schema.Green
{
    public class ComplexTypeSchemaGreen : TypeSchemaGreen
    {
        public ComplexTypeSchemaGreen(Option<Type> dotNetType, string schemaName, string codeName, string contractName,
            int? baseTypeId, bool isStruct, ImmutableDictionary<string, int> fields) : base(dotNetType, false)
        {
            SchemaName = schemaName;
            CodeName = codeName;
            ContractName = contractName;
            BaseTypeId = baseTypeId;
            IsStruct = isStruct;
            Fields = fields;
        }

        public ComplexTypeSchemaGreen(TypeSchemaGreen @base, Option<Type> dotNetType, string schemaName,
            string codeName, string contractName, int? baseTypeId, bool isStruct,
            ImmutableDictionary<string, int> fields) : base(@base, dotNetType, false)
        {
            SchemaName = schemaName;
            CodeName = codeName;
            ContractName = contractName;
            BaseTypeId = baseTypeId;
            IsStruct = isStruct;
            Fields = fields;
        }

        public string SchemaName { get; }
        public string CodeName { get; }
        public string ContractName { get; }
        public int? BaseTypeId { get; }
        public bool IsStruct { get; }
        public ImmutableDictionary<string, int> Fields { get; }
    }
}