using System;
using System.Linq.Expressions;
using Astral;

namespace RabbitLink.Services
{
    /// <summary>
    /// service builder
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    public interface IServiceBuilder<TService>
    {
        /// <summary>
        /// event endpoint factory
        /// </summary>
        /// <param name="selector">event property selector</param>
        /// <typeparam name="TEvent">event contract type</typeparam>
        /// <returns>event endpoint</returns>
        IEventEndpoint<TService, TEvent> Event<TEvent>(Expression<Func<TService, EventHandler<TEvent>>> selector); 
            
        
        /// <summary>
        /// call endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg> Call<TArg>(Expression<Func<TService, Action<TArg>>> selector);


        /// <summary>
        /// call endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <typeparam name="TResult">call result contract type</typeparam>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg, TResult> Call<TArg, TResult>(
            Expression<Func<TService, Func<TArg, TResult>>> selector);


        /// <summary>
        /// request endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <typeparam name="TResult">call result contract type</typeparam>
        /// <returns>request endpoint</returns>
        IRequestEndpoint<TService, TArg, TResult> Request<TArg, TResult>(
            Expression<Func<TService, Func<TArg, TResult>>> selector);


        /// <summary>
        /// request endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <returns>request endpoint</returns>
        IRequestEndpoint<TService, TArg, RpcOk> Request<TArg>(Expression<Func<TService, Action<TArg>>> selector);


        /// <summary>
        /// response endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <typeparam name="TResult">call result contract type</typeparam>
        /// <returns>response endpoint</returns>
        IResponseEndpoint<TService, TArg, TResult> Response<TArg, TResult>(
            Expression<Func<TService, Func<TArg, TResult>>> selector);
        
        /// <summary>
        /// response endpoint factory
        /// </summary>
        /// <param name="selector">call property selector</param>
        /// <typeparam name="TArg">call argument contract type</typeparam>
        /// <returns>response endpoint</returns>
        IResponseEndpoint<TService, TArg, RpcOk> Response<TArg>(
            Expression<Func<TService, Action<TArg>>> selector);

    }

    
}