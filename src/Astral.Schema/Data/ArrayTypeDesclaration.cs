using System;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public sealed class ArrayTypeDesclaration : IArrayTypeDeclarationSchema
    {
        public ServiceSchema Service { get; }
        private ArrayTypeDescriptionSchemaGreen Green { get; }

        public ArrayTypeDesclaration(ServiceSchema service, ArrayTypeDescriptionSchemaGreen green)
        {
            Service = service;
            Green = green;
        }

        public string Name => ElementType.Name + "[]";


        public string CodeName => ElementType.CodeName + "[]";


        public string Code => ElementType.Code + "[]";


        public Type DotNetType => Green.DotNetType;


        public bool IsWellKnown => true;


        public ITypeDeclarationSchema ElementType => Service.TypeById(Green.ElementId);

    }
}