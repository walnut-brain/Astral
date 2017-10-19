using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Astral.Schema.Green
{
    public abstract class TypeSchemaGreen : GreenNode
    {
        protected TypeSchemaGreen(Option<Type> dotNetType, bool isWellKnown)
        {
            DotNetType = dotNetType;
            IsWellKnown = isWellKnown;
        }

        protected TypeSchemaGreen(TypeSchemaGreen @base, Option<Type> dotNetType, bool isWellKnown) : base(@base)
        {
            DotNetType = dotNetType;
            IsWellKnown = isWellKnown;
        }


        public Option<Type> DotNetType { get; }
        public bool IsWellKnown { get; }

        
    }

}