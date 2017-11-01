using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Astral.Logging;
using Astral.Schema;

namespace Astral.Liaison
{
    public static class Extensions
    {

        private class DelegateConsumer<T> : IConsumer<T>
        {
            private Func<Func<T, CancellationToken, Task<Acknowledge>>, IDisposable> _listen;

            public DelegateConsumer(IEndpointSchema schema, Func<Func<T, CancellationToken, Task<Acknowledge>>, IDisposable> listen)
            {
                _listen = listen;
                Schema = schema;
            }


            public IEndpointSchema Schema { get; }


            public IDisposable Listen(Func<T, CancellationToken, Task<Acknowledge>> listener)
                => _listen(listener);
        }

        public static IConsumer<T> Parallelism<T>(this IConsumer<T> consumer, int parallelism = 1)
        {
            var logFactory =
                consumer is ILogFactoryProvider provider ? provider.LogFactory : new FakeLogFactory();
            var logger = logFactory.CreateLog("Parallelism");
            
            return new DelegateConsumer<T>(consumer.Schema, listener =>
            {
                var l = logger.With("{parallelism}", parallelism);
                l.Trace("Listen");
                var buffer = new BufferBlock<IAck<(T, CancellationToken)>>();

                var action = new ActionBlock<IAck<(T, CancellationToken)>>(async p =>
                {
                    
                    var (m, ct) = p.Value;
                    var nl = l.With("{@message}", m);
                    nl.Trace("Message processing");
                    try
                    {
                        var acknowledge = await listener(m, ct);
                        p.SetResult(acknowledge);
                        nl.With("{acknowledge}", acknowledge).Trace("Message processed");
                    }
                    catch (Exception ex)
                    {
                        p.SetError(ex);
                        nl.Warn("Message processed", ex);
                    }

                },new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = parallelism });

                var link = buffer.LinkTo(action);
                
                var disposable = consumer.Listen(async (msg, ct) =>
                {
                    var nl = l.With("{@message}", msg);
                    nl.Trace("Bufferring");
                    var source = new TaskCompletionSource<Acknowledge>();

                    if (!await buffer.SendAsync(new Ack<(T, CancellationToken)>((msg, ct), p => source.TrySetResult(p),
                        p => source.TrySetException(p)), ct))
                        return Acknowledge.Requeue;
                    return await source.Task;
                });

                return new CompositeDisposable(disposable, link, Disposable.Create(() => buffer.Complete()), Disposable.Create(() => action.Complete()));                
            });
        }
        
        
        
        public static IConsumer<T> Where<T>(this IConsumer<T> consumer, Func<T, bool> filter,
            Func<T, Task<Acknowledge>> missed = null)
        {
            missed = missed ?? (p => Task.FromResult(Acknowledge.Nack));
            
            return new DelegateConsumer<T>(consumer.Schema, listener =>
            {
                return consumer.Listen(async (p, ct) =>
                {
                    if (filter(p))
                        return await listener(p, ct);
                    return await missed(p);
                });
            });
        }
        
        
        public static IObservable<IAck<T>> ToObservable<T>(this IConsumer<T> consumer)
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