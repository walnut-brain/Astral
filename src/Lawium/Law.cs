using System;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    /// <summary>
    /// Inference law
    /// </summary>
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

        /// <summary>
        /// Name of law
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// In arguments types
        /// </summary>
        public Arr<Type> Arguments { get; }

        /// <summary>
        /// Result of inference types
        /// </summary>
        public Arr<Type> Findings { get; }


        internal Func<ILogger, Arr<object>, Arr<object>> Executor { get; }
        
        /// <summary>
        /// Create axiom law
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="value">value</param>
        /// <returns>law inference axiom value for type</returns>
        public static Law Axiom<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new Law($"Value of {typeof(T)}", Prelude.Array<Type>(),
                Prelude.Array(typeof(T)), (log, args) => Prelude.Array<object>(value));
        }

        
    }
}