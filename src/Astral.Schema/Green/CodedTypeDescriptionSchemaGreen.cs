using System;
using System.Collections.Generic;

namespace Astral.Schema.Green
{
    public abstract class CodedTypeDescriptionSchemaGreen : TypeDescriptionSchemaGreen

    {
        protected CodedTypeDescriptionSchemaGreen(int id, string name, Type dotNetType, bool isWellKnown, string code) :
            base(id, name, dotNetType, isWellKnown)
        {
            Code = code;
        }

        public string Code { get; }
    }

    
}