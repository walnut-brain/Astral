using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Lawium
{
    /// <summary>
    /// Inference law
    /// </summary>
    public partial class Law<T>
    {

        /// <summary>
        /// Create law from delegate
        /// </summary>
        /// <param name="name">name of law</param>
        /// <param name="delegate">delegate to use</param>
        /// <returns>new law</returns>
        /// <exception cref="ArgumentException">when delegate has bad signature</exception>
        public static Law<T> FromDelegate(string name, Delegate @delegate)
        {
            var parameterTypes = @delegate.Method.GetParameters().Select(p => p.ParameterType).ToImmutableArray();
            if(parameterTypes.GroupBy(p => p).Select(p => (p.Key, p.Count())).Any(t => t.Item2 > 1))
                throw new ArgumentException("Has same types in parameter list");
            var returnType = @delegate.Method.ReturnType;
            if(returnType.IsValueType)
            {
                var resultTypes = GetTupleTypes(returnType).ToList();
                if(resultTypes.GroupBy(p => p).Select(p => (p.Key, p.Count())).Any(t => t.Item2 > 1))
                    throw new ArgumentException("Has same types in return list");
                return new Law<T>(name, parameterTypes.ToImmutableArray(), resultTypes.ToImmutableArray(),
                    (_, prm) => TupleToArray(@delegate.DynamicInvoke(prm.ToArray())).ToImmutableArray());
            }
            return new Law<T>(name, parameterTypes.ToImmutableArray(), ImmutableArray.Create(returnType),
                (_, prm) => ImmutableArray.Create(@delegate.DynamicInvoke(prm.ToArray())));

            IEnumerable<Type> GetTupleTypes(Type type)
            {
                if (type.GenericTypeArguments.Length < 8)
                    return type.GenericTypeArguments;
                return type.GenericTypeArguments.Take(7).Union(GetTupleTypes(type.GenericTypeArguments[7]));
            }

            IEnumerable<object> TupleToArray(object tuple)
            {
                var type = tuple.GetType();
                for (var i = 0; i < type.GenericTypeArguments.Length; i++)
                {
                    if (i < 7)
                        yield return type.GetProperty("Item" + (i + 1)).GetValue(tuple);
                    else
                        foreach (var o in TupleToArray(type.GetProperty("Item" + (i + 1)).GetValue(tuple)))
                            yield return o;
                }
            }
        }
        
        

        internal Law(string name, ImmutableArray<Type> arguments, ImmutableArray<Type> findings,
            Func<ILogger, ImmutableArray<object>, ImmutableArray<object>> executor) 
        {
        
            Name = name;
            var badArguments = arguments.Where(p => !typeof(T).IsAssignableFrom(p)).ToList();
            if(badArguments.Count > 0)
                throw new AggregateException($"Arguments array contains types not assignable to {typeof(T)}",
                    badArguments.Select(p => new ArgumentException($"Used incompatible argument type {p}")));
            var badFindings = findings.Where(p => !typeof(T).IsAssignableFrom(p)).ToList();
            if(badFindings.Count > 0)
                throw new AggregateException($"Result type array contains types not assignable to {typeof(T)}",
                    badFindings.Select(p => new ArgumentException($"Used incompatible argument type {p}")));
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
        /// <typeparam name="TKey">type of result</typeparam>
        /// <param name="value">value</param>
        /// <returns>law inference axiom value for type</returns>
        public static Law<T> Axiom<TKey>(TKey value)
            where TKey : T
        {
            if (Equals(value, null)) throw new ArgumentNullException(nameof(value));
            return new Law<T>($"Value of {typeof(TKey)}", ImmutableArray<Type>.Empty,
                ImmutableArray.Create(typeof(TKey)), (log, args) => ImmutableArray.Create((object) value));
        }

        /// <summary>
        /// Create axiom law
        /// </summary>
        /// <param name="type">type of result</param>
        /// <param name="value">value</param>
        /// <returns>law inference axiom value for type</returns>
        public static Law<T> Axiom(Type type, object value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if(!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not assignable to {typeof(T)}");
            if(!type.IsInstanceOfType(value))
                throw new ArgumentException($"{value} is not instance of {type}");
            return new Law<T>($"Value of {type}", ImmutableArray<Type>.Empty, 
                ImmutableArray.Create(type), (log, args) => ImmutableArray.Create(value));
        }
        
    }
}