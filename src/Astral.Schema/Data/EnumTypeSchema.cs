using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public sealed class EnumTypeSchema : IEnumTypeDeclarationSchema
    {
        private readonly EnumTypeSchemaGreen _green;
        public ServiceSchema Service { get; }

        public EnumTypeSchema(ServiceSchema service, EnumTypeSchemaGreen green)
        {
            _green = green;
            Service = service;
        }

        public string ContractName => _green.ContractName;


        public string CodeName => _green.CodeName;

        public ServiceSchema SetCodeName(string codeName)
        {
            var newTypes = Service.Green.Types.SetItem(_green.Id, new EnumTypeSchemaGreen(_green, _green.DotNetType,
                _green.SchemaName,
                codeName, _green.ContractName, _green.BaseTypeId, _green.IsFlags, _green.Values));
            return new ServiceSchema(new ServiceSchemaGreen(Service.Green.Name, Service.Green.Owner, 
                Service.Green.CodeName, Service.Green.Events, Service.Green.Calls, newTypes, Service.Green.ContentType,
                Service.Green.Exchange, Service.Green.ResponseExchange));
        }
        

        public string SchemaName => _green.SchemaName;


        public Type DotNetType => _green.DotNetType.IfNoneDefault();
        
        public ServiceSchema SetDotNetType(Type type)
        {
            var newTypes = Service.Green.Types.SetItem(_green.Id, new EnumTypeSchemaGreen(_green, type,
                _green.SchemaName,
                _green.CodeName, _green.ContractName, _green.BaseTypeId, _green.IsFlags, _green.Values));
            return new ServiceSchema(new ServiceSchemaGreen(Service.Green.Name, Service.Green.Owner, 
                Service.Green.CodeName, Service.Green.Events, Service.Green.Calls, newTypes, Service.Green.ContentType,
                Service.Green.Exchange, Service.Green.ResponseExchange));
        }


        public bool IsWellKnown => false;


        public ITypeDeclarationSchema BaseOn => Service.TypeById(_green.BaseTypeId);

        public bool IsFlags => _green.IsFlags;


        public IReadOnlyDictionary<string, long> Values => _green.Values;

    }
}