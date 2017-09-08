using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Transactions;
using FunEx.TypeClasses;

namespace FunEx.Monads
{

    public delegate (T, TAcc) Writer<T, TMonoid, TAcc>()
        where TMonoid : struct, SMonoid<TAcc>; 
    
    public static class Writer
    {
        public interface IWriterBuilder<T, TAcc>
        {
            Writer<T, TMonoid, TAcc> WithMonoid<TMonoid>() where TMonoid : struct, SMonoid<TAcc>;
        }

        public static IWriterBuilder<T, TAcc> Create<T, TAcc>(Func<(T, TAcc)> func)
            => new WriterBuilder<T, TAcc>(func);
        
        public static IWriterBuilder<T, TAcc> Create<T, TAcc>((T, TAcc) tuple)
            => new WriterBuilder<T, TAcc>(() => tuple);
        
        public static IWriterBuilder<T, TAcc> Create<T, TAcc>(T value, TAcc acc)
            => new WriterBuilder<T, TAcc>(() => (value, acc));


        public static Writer<TResult, TMonoid, TAcc> Bind<TSource, TMonoid, TAcc, TResult>(
            this Writer<TSource, TMonoid, TAcc> source,
            Func<TSource, Writer<TResult, TMonoid, TAcc>> binder) where TMonoid : struct, SMonoid<TAcc>
            => () =>
                {
                    var (s, acc1) = source();
                    var (t, acc2) = binder(s)();
                    return (t, default(TMonoid).Append(acc1, acc2));
                };



        public static Writer<TResult, TMonoid, TAcc> Map<TSource, TMonoid, TAcc, TResult>(
            this Writer<TSource, TMonoid, TAcc> source,
            Func<TSource, TResult> mapper) where TMonoid : struct, SMonoid<TAcc>
            => () =>
                {
                    var (s, acc1) = source();
                    return (mapper(s), acc1);
                };

        public static Writer<T, TMonoid, TAcc> Write<T, TMonoid, TAcc>(this Writer<T, TMonoid, TAcc> writer, Func<T, TAcc> acc) 
            where TMonoid : struct, SMonoid<TAcc> 
            => writer.Bind(p => Create(p, acc(p)).WithMonoid<TMonoid>());
        
        public static Writer<T, TMonoid, TAcc> Write<T, TMonoid, TAcc>(this Writer<T, TMonoid, TAcc> writer, TAcc acc) 
            where TMonoid : struct, SMonoid<TAcc> 
            => writer.Bind(p => Create(p, acc).WithMonoid<TMonoid>());
        
        


        public static Writer<T, TMonoid, TAcc> Unwrap<T, TMonoid, TAcc>(
            this Writer<Writer<T, TMonoid, TAcc>, TMonoid, TAcc> writer)
            where TMonoid : struct, SMonoid<TAcc>
            => writer.Bind(p => p);
        
        public static Func<Writer<TResult, TMonoid, TAcc>> Combine<TMonoid, TAcc, TSource, TResult>(
            this Func<Writer<TSource, TMonoid, TAcc>> f1, Func<TSource, Writer<TResult, TMonoid, TAcc>> f2) 
            where TMonoid : struct, SMonoid<TAcc> => () => f1().Bind(f2);
        
        public static Func<TSource, Writer<TResult, TMonoid, TAcc>> Combine<TSource, TMonoid, TAcc, TMiddle, TResult>(
            this Func<TSource, Writer<TMiddle, TMonoid, TAcc>> f1, Func<TMiddle, Writer<TResult, TMonoid, TAcc>> f2) 
            where TMonoid : struct, SMonoid<TAcc> => source => f1(source).Bind(f2);
        

        private class WriterBuilder<T, TAcc> : IWriterBuilder<T, TAcc>
        {
            private readonly Func<(T, TAcc)> _func;

            public WriterBuilder(Func<(T, TAcc)> func)
            {
                _func = func;
            }

            public Writer<T, TMonoid, TAcc> WithMonoid<TMonoid>() where TMonoid : struct, SMonoid<TAcc>
                => () => _func();
        }
    }
}