using System;
using System.Collections.Generic;

namespace CsFun
{
    public struct Option<T>
    {
        private readonly T _value;

        public Option(T value)
        {
            if (!Equals(value, null))
            {
                IsSome = true;
                _value = value;    
            }
            else
            {
                IsSome = false;
                _value = default(T);
            }
        }

        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        public void Match(Action<T> onSome, Action onNone)
        {
            if(IsSome)  
                onSome(_value);
            else
                onNone();
        }

        public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            => IsSome ? onSome(_value) : onNone();
        

        public static implicit operator Option<T>(OptionNone none)
            => default(Option<T>);

        public bool Equals(Option<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) && IsSome == other.IsSome;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Option<T> option && Equals(option);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ IsSome.GetHashCode();
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }
    }

    public struct OptionNone
    {
        public static readonly OptionNone None = new OptionNone(); 
    }

    public static class Option
    {
        public static Option<T> ToOption<T>(this T value) => new Option<T>(value);

        public static Option<TResult> Map<T, TResult>(this Option<T> source, Func<T, TResult> mapper)
            => source.Match(p => mapper(p).ToOption(), () => OptionNone.None);

        public static Option<TResult> Bind<T, TResult>(this Option<T> source, Func<T, Option<TResult>> binder)
            => source.Match(binder, () => OptionNone.None);

        public static Option<T> OrElse<T>(this Option<T> source, Func<Option<T>> fallback)
            => source.Match(p => p.ToOption(), fallback);

        public static T IfNone<T>(this Option<T> source, Func<T> onNone)
            => source.Match(p => p, onNone);

        public static void IfNone<T>(this Option<T> source, Action onNone)
            => source.Match(_ => { }, onNone);

        public static void IfSome<T>(this Option<T> source, Action<T> onSome)
            => source.Match(onSome, () => { });

        public static Option<T> Filter<T>(this Option<T> source, Func<T, bool> condition)
            => source.Bind(p => condition(p) ? p.ToOption() : OptionNone.None);
        
        public static OptionNone None => OptionNone.None;

        public static T Unwrap<T>(this Option<T> source, Exception ex = null)
        {
            ex = ex ?? new InvalidOperationException("Unwrap None");
            return source.Match(p => p, () => throw ex);
        }
        
        public static Result<T> ToResult<T>(this Option<T> source, Exception ex = null)
        {
            ex = ex ?? new NoResultException();
            return source.Match(p => p.ToSuccess(), () => new NoResultException().ToFail<T>());
        }

        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
        
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
        
        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
    }
}