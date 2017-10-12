using System;
using System.Collections.Generic;

namespace Astral.Schema.Green
{
    public class ArrayTypeDescriptionSchemaGreen : TypeDescriptionSchemaGreen
    {
        public ArrayTypeDescriptionSchemaGreen(Type dotNetType, int elementId) : base(NextId, null, dotNetType, true)
        {
            ElementId = elementId;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ArrayTypeDescriptionSchemaGreen(ArrayTypeDescriptionSchemaGreen schema, Type dotNetType, int elementId) : base(schema.Id, null, dotNetType, true)
        {
            ElementId = elementId;
        }

        public int ElementId { get; };

       
    }

    
    
}