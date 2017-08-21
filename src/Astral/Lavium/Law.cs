using System;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Prelude = Astral.Lavium.Prelude;

namespace Astral.Lavium
{
    public class Law
    {
        internal Law(string name, Arr<Type> arguments, Arr<Type> findings,
            Func<ILogger, Func<Type, object>, Arr<object>, Arr<object>> executor) 
        {
            Id = Prelude.NextLawId; 
            Name = name;
            Arguments = arguments;
            Findings = findings;
            Executor = executor;
        }


        public int Id { get; }
    
        
        public string Name { get; }
        public Arr<Type> Arguments { get; }
        public Arr<Type> Findings { get; }
        public Func<ILogger, Func<Type, object>, Arr<object>, Arr<object>> Executor { get; }
        
        
        public Law Map(Func<Law, Func<ILogger, Func<Type, object>, Arr<object>, Arr<object>>> mapper)
            => new Law(Name, Arguments, Findings, mapper(this));
    }
}