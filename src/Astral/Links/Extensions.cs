using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Astral.Links
{
    public static class Extensions
    {
        public static IObservable<IAck<T>> ToObservable<T>(this IEventConsumer<T> consumer)
        {
            return Observable.Create<IAck<T>>(observer =>
            {
                var buffer = new BufferBlock<IAck<T>>();
                var compositeDisposable = new CompositeDisposable
                {
                    consumer.Listen((p, ct) =>
                    {
                        var source = new TaskCompletionSource<Acknowledge>();
                        var ack = new Ack<T>(p, a => source.TrySetResult(a), ex => source.TrySetException(ex));
                        buffer.Post(ack);
                        return source.Task;
                    }),
                    buffer.AsObservable().Subscribe(observer)
                };
                return compositeDisposable;
            });
        }
        
        /// <summary>
        /// Select with index
        /// </summary>
        /// <typeparam name="TSource">source message type</typeparam>
        /// <typeparam name="TResult">result message type</typeparam>
        /// <param name="observable">this</param>
        /// <param name="selector">selector</param>
        /// <returns>observable</returns>
        public static IObservable<IAck<TResult>> Select<TSource, TResult>(this IObservable<IAck<TSource>> observable,
            Func<TSource, int, TResult> selector)
        {
            return observable.Select((p, i) =>
            {
                try
                {
                    return new Ack<TResult>(selector(p.Value, i), p.SetResult, p.SetError);
                }
                catch (Exception e)
                {
                    p.SetError(e);
                    throw;
                }
            });
        }
        
        /// <summary>
        /// Select  
        /// </summary>
        /// <typeparam name="TSource">source message type</typeparam>
        /// <typeparam name="TResult">result message type</typeparam>
        /// <param name="observable">this</param>
        /// <param name="selector">selector</param>
        /// <returns>observable</returns>
        public static IObservable<IAck<TResult>> Select<TSource, TResult>(this IObservable<IAck<TSource>> observable,
            Func<TSource, TResult> selector)
        {
            return observable.Select(p =>
            {
                try
                {
                    return new Ack<TResult>(selector(p.Value), p.SetResult, p.SetError);
                }
                catch (Exception e)
                {
                    p.SetError(e);
                    throw;
                }
            });
        }

        /// <summary>
        /// Filter messages
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="observable">this</param>
        /// <param name="filter">filter</param>
        /// <param name="others">how process other messages, default nack</param>
        /// <returns>observable</returns>
        public static IObservable<IAck<T>> Where<T>(this IObservable<IAck<T>> observable,
            Func<T, bool> filter, Action<IAck<T>> others = null)
        {
            return observable.Where(p =>
            {
                try
                {
                    var flt = filter(p.Value);
                    if(!flt)
                        if(others == null)
                            p.Nack();
                        else
                            others(p);
                    return flt;
                }
                catch (Exception e)
                {
                    p.SetError(e);
                    throw;
                }
            });
        }

        public static IObservable<IAck<T>> ByMax<T, TResult>(this IObservable<IAck<T>> observable,
            Func<T, TResult> selector, Action<IAck<T>> complete)
        {
            return Observable.Create<IAck<T>>(obs =>
            {
                IAck<T> current = null;
                return observable.Subscribe(p =>
                    {
                        if (current == null)
                        {
                            current = p;
                        }
                        else
                        {
                            if (Comparer<TResult>.Default.Compare(selector(current.Value), selector(p.Value)) > 0)
                            {
                                complete(p);
                            }
                            else
                            {
                                complete(current);
                                current = p;
                            }
                        }
                    }, obs.OnError,
                    () =>
                    {
                        if (current != null)
                            obs.OnNext(current);
                        obs.OnCompleted();
                    }
                );
            });
        }

        
    }
}