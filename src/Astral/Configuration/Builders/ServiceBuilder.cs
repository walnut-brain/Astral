using System;
using System.Linq;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class ServiceBuilder : BuilderBase
    {
        protected ServiceBuilder(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider, bookBuilder)
        {
        }
    }

    public class ServiceBuilder<TService> : ServiceBuilder
    {
        internal ServiceBuilder(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider, bookBuilder)
        {
        }

        public EventEndpointBuilder<TEvent> Endpoint<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b => b.AddEndpointLaws(propertyInfo));
            return new EventEndpointBuilder<TEvent>(ServiceProvider, builder);
        }

        public CallEndpointBuilder<TArgs> Endpoint<TArgs>(Expression<Func<TService, ICall<TArgs>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b => b.AddEndpointLaws(propertyInfo));
            return new CallEndpointBuilder<TArgs>(ServiceProvider, builder);
        }

        public CallEndpointBuilder<TArgs, TResult> Endpoint<TArgs, TResult>(
            Expression<Func<TService, ICall<TArgs, TResult>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b => b.AddEndpointLaws(propertyInfo));
            return new CallEndpointBuilder<TArgs, TResult>(ServiceProvider, builder);
        }
    }
}