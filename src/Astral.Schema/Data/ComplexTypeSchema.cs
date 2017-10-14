﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public class ComplexTypeSchema : IComplexTypeSchema
    {
        private readonly ComplexTypeSchemaGreen _green;
        private Lazy<ImmutableDictionary<string, ITypeSchema>> _lazyFields;
        public ServiceSchema Service { get; }

        public ComplexTypeSchema(ServiceSchema service, ComplexTypeSchemaGreen green)
        {
            _green = green;
            Service = service;
            _lazyFields = new Lazy<ImmutableDictionary<string, ITypeSchema>>(
                () =>
                {
                    var builder = ImmutableDictionary.CreateBuilder<string, ITypeSchema>();
                    builder.AddRange(_green.Fields.Select(p =>
                        new KeyValuePair<string, ITypeSchema>(p.Key, Service.TypeById(p.Value))));
                    return builder.ToImmutable();
                });
        }

        public string ContractName => _green.ContractName;


        public string CodeName => _green.CodeName;

        public ServiceSchema SetCodeName(string value)
        {
            var newTypes = Service.Green.Types.SetItem(_green.Id, new ComplexTypeSchemaGreen(_green, _green.DotNetType,
                _green.SchemaName, value,
                _green.ContractName, _green.BaseTypeId, _green.IsStruct, _green.Fields));
            return new ServiceSchema(new ServiceSchemaGreen(Service.Green.Name, Service.Green.Owner, Service.Green.CodeName,
                Service.Green.Events, Service.Green.Calls, newTypes, Service.Green.ContentType, Service.Green.Exchange,
                Service.Green.ResponseExchange));
        }


        public string SchemaName => _green.SchemaName;


        public Type DotNetType => _green.DotNetType.IfNoneDefault();


        public bool IsWellKnown => false;


        public bool IsStruct => _green.IsStruct;


        public IComplexTypeSchema BaseOn =>
             _green.BaseTypeId == null ? null : (IComplexTypeSchema) Service.TypeById(_green.BaseTypeId.Value);



        public IReadOnlyDictionary<string, ITypeSchema> Fields => _lazyFields.Value;

    }
}