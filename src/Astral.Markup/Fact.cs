using System;
using System.Collections.Generic;

namespace Astral.Markup
{
    /// <summary>
    /// Implementation of base class for typed Law values
    /// </summary>
    /// <typeparam name="T">type of value</typeparam>
    public abstract class Fact<T> 
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">value of fact</param>
        protected Fact(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of fact
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// implicit conversion from Fact&lt;T&gt; to T
        /// </summary>
        /// <param name="fact">fact</param>
        /// <returns>Value of fact</returns>
        public static implicit operator T(Fact<T> fact) => fact.Value;

        protected bool Equals(Fact<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Fact<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        /// <summary>
        /// Equality comparasion
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>equality</returns>
        public static bool operator ==(Fact<T> left, Fact<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not equality comparasion
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>not equality</returns>
        public static bool operator !=(Fact<T> left, Fact<T> right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Fact with predicate
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    /// <typeparam name="TPred">predicate</typeparam>
    public abstract class Fact<T, TPred> : Fact<T>
        where TPred : struct, IPredicate<T>
    {
        /// <inheritdoc />
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">fact value</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">when value not conform predicate</exception>
        protected Fact(T value) : base(value)
        {
            var pred = default(TPred);
            var (check, error) = pred.True(value);
            if (!check)
                throw new ArgumentOutOfRangeException(error ?? $"Value {value} not conform {pred}");
        }
    }
}