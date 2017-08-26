using System;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    public partial class Law
    {

        internal Law(string name, Arr<Type> arguments, Arr<Type> findings,
            Func<ILogger, Arr<object>, Arr<object>> executor) 
        {
        
            Name = name;
            Arguments = arguments;
            Findings = findings;
            Executor = executor;
        }

        
        public string Name { get; }
        public Arr<Type> Arguments { get; }
        public Arr<Type> Findings { get; }
        public Func<ILogger, Arr<object>, Arr<object>> Executor { get; }
        
        
        public Law Map(Func<Law, Func<ILogger, Arr<object>, Arr<object>>> mapper)
            => new Law(Name, Arguments, Findings, mapper(this));

        public static Law Axiom<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new Law($"Value of {typeof(T)}", Prelude.Array<Type>(),
                Prelude.Array(typeof(T)), (log, args) => Prelude.Array<object>(value));
        }

        
    }
}