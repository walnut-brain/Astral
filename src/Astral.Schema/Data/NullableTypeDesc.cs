using System;

namespace Astral.Schema.Data
{
    public class NullableTypeDesc : TypeDesc
    {
        public NullableTypeDesc(Type dotNetType, TypeDesc @base)
        {
            Base = @base;
            DotNetType = dotNetType;
        }

        public TypeDesc Base { get; }

        public override string Contract => Base.Contract + "?";
        

        public override Type DotNetType { get; }
        
    }
}