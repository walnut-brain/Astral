using System;
using System.Collections.Generic;

namespace Astral.Schema.Green
{
    public class ArrayTypeSchemaGreen : TypeSchemaGreen
    {
        public ArrayTypeSchemaGreen(Option<Type> dotNetType, int elementId) : base(dotNetType, true)
        {
            ElementId = elementId;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ArrayTypeSchemaGreen(TypeSchemaGreen schema, Option<Type> dotNetType, int elementId) : base(schema, dotNetType, true)
        {
            ElementId = elementId;
        }

        public int ElementId { get; }
    }

    
    
}