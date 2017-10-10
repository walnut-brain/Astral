using System;

namespace Astral.Schema.Data
{
    public sealed class ArrayTypeDesc : TypeDesc
    {
        public ArrayTypeDesc(TypeDesc elementType, Type dotNetType)
        {
            ElementType = elementType;
            DotNetType = dotNetType;
        }

        public TypeDesc ElementType { get; }

        public override string Contract => ElementType.Contract + "[]";
        public override Type DotNetType { get; }
    }
}