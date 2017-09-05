using System;
using System.Collections.Generic;

namespace CsFun
{
    public struct Result<T>
    {
        private readonly T _value;
        private readonly Exception _error;

        private Exception Error => _error ?? new NoResultException();

        public Result(T value) : this()
        {
            IsSuccess = true;
            _value = value;
            _error = null;
        }

        public Result(Exception error) : this()
        {
            _error = error;
            IsSuccess = false;
            _value = default(T);
        }

        public void Match(Action<T> onSuccess, Action<Exception> onError)
        {
            if (IsSuccess)
                onSuccess(_value);
            else
                onError(Error);
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Exception, TResult> onError)
            => IsSuccess ? onSuccess(_value) : onError(Error);

        public bool Equals(Result<T> other)
        {
            return IsSuccess == other.IsSuccess && EqualityComparer<T>.Default.Equals(_value, other._value) && Equals(_error, other._error);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result<T> result && Equals(result);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsSuccess.GetHashCode();
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
                hashCode = (hashCode * 397) ^ (_error != null ? _error.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Result<T> left, Result<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<T> left, Result<T> right)
        {
            return !left.Equals(right);
        }

        public bool IsSuccess { get; }

        public bool IsFail => !IsSuccess;
    }

    public class NoResultException : Exception
    {
        
    }

    public static class Result
    {
        public static Result<T> Try<T>(Func<T> op)
        {
            try
            {
                return op().ToSuccess();
                
            }
            catch (Exception ex)
            {
                return ex.ToFail<T>();
            }
        }
        
        public static Result<T> ToSuccess<T>(this T value) => new Result<T>(value);
        public static Result<T> ToFail<T>(this Exception ex) => new Result<T>(ex);

        public static Result<TResult> Bind<T, TResult>(this Result<T> source, Func<T, Result<TResult>> binder)
            => source.Match(p => Try(() => binder(p)).Unwrap(), ex => ex.ToFail<TResult>());
        
        public static Result<TResult> Map<T, TResult>(this Result<T> source, Func<T, TResult> mapper)
            => source.Match(p => Try(() => mapper(p)), ex => ex.ToFail<TResult>());

        public static Result<T> OrElse<T>(this Result<T> source, Func<Result<T>> fallback)
            => source.Match(p => p.ToSuccess(), _ => Try(fallback).Unwrap());
        
        public static Result<TResult> BiBind<T, TResult>(this Result<T> source, Func<T, Result<TResult>> bindSuccess,
            Func<Exception, Result<TResult>> bindFail)
            => source.Match(p => Try(() => bindSuccess(p)).Unwrap(), ex => Try(() => bindFail(ex)).Unwrap());

        public static Result<T> Unwrap<T>(this Result<Result<T>> source)
            => source.Match(p => p, ex => ex.ToFail<T>());
        
        public static T Unwrap<T>(this Result<T> source)
            => source.Match(p => p, ex => throw ex);

        public static T IfFail<T>(this Result<T> source, Func<Exception, T> onFail)
            => source.Match(p => p, onFail);
        
        public static void IfFail<T>(this Result<T> source, Action<Exception> onFail)
            => source.Match(p => { }, onFail);
        
        public static void IfSucces<T>(this Result<T> source, Action<T> onSucces)
            => source.Match(onSucces, _ => { });

        public static Option<T> ToOption<T>(this Result<T> source)
            => source.Match(p => p.ToOption(), _ => Option.None);

    }
    
    
}