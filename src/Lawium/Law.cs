using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    /// <summary>
    /// Inference law
    /// </summary>
    public partial class Law
    {

        internal Law(string name, ImmutableArray<Type> arguments, ImmutableArray<Type> findings,
            Func<ILogger, ImmutableArray<object>, ImmutableArray<object>> executor) 
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
        public ImmutableArray<Type> Arguments { get; }

        /// <summary>
        /// Result of inference types
        /// </summary>
        public ImmutableArray<Type> Findings { get; }


        internal Func<ILogger, ImmutableArray<object>, ImmutableArray<object>> Executor { get; }
        
        /// <summary>
        /// Create axiom law
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="value">value</param>
        /// <returns>law inference axiom value for type</returns>
        public static Law Axiom<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new Law($"Value of {typeof(T)}", ImmutableArray<Type>.Empty,
                ImmutableArray.Create(typeof(T)), (log, args) => ImmutableArray.Create((object) value));
        }

        /// <summary>
        /// Create axiom law
        /// </summary>
        /// <param name="type">type of result</param>
        /// <param name="value">value</param>
        /// <returns>law inference axiom value for type</returns>
        public static Law Axiom(Type type, object value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if(!type.IsInstanceOfType(value))
                throw new ArgumentException($"{value} is not instance of {type}");
            return new Law($"Value of {type}", ImmutableArray<Type>.Empty, 
                ImmutableArray.Create(type), (log, args) => ImmutableArray.Create(value));
        }
        
    }
}