using System;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class ServiceBuilder : BuilderBase
    {
        protected ServiceBuilder(ILoggerFactory factory, LawBookBuilder bookBuilder) : base(factory, bookBuilder)
        {
        }
    }

    public class ServiceBuilder<TService> : ServiceBuilder
    {
        internal ServiceBuilder(ILoggerFactory factory, LawBookBuilder bookBuilder) : base(factory, bookBuilder)
        {
        }

        public EventEndpointBuilder<TEvent> Endpoint<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Event));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TEvent))));
                });
            return new EventEndpointBuilder<TEvent>(LoggerFactory, builder);

        }
    }
}