using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Astral.Schema.Green
{
    public abstract class TypeDescriptionSchemaGreen 
    {
        protected TypeDescriptionSchemaGreen(int id, string name, Type dotNetType, bool isWellKnown)
        {
            Id = id;
            Name = name;
            DotNetType = dotNetType;
            IsWellKnown = isWellKnown;
        }

        public int Id { get; }
        public string Name { get; }
        public Type DotNetType { get; }
        public bool IsWellKnown { get; }

        public static int NextId => Interlocked.Increment(ref _idGen);

        private static int _idGen;
    }

}