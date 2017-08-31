﻿using System;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Astral.Markup;
using Lawium;
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

        public CallEndpointBuilder<TArgs> Endpoint<TArgs>(Expression<Func<TService, ICall<TArgs>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Call));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TArgs))));
                    b.RegisterLaw(Law.Axiom(new ResponseType(typeof(ValueTuple))));
                });
            return new CallEndpointBuilder<TArgs>(LoggerFactory, builder);
        }

        public CallEndpointBuilder<TArgs, TResult> Endpoint<TArgs, TResult>(
            Expression<Func<TService, ICall<TArgs, TResult>>> selector)
        {
            var propertyInfo = selector.GetProperty();
            var builder = BookBuilder.GetSubBookBuilder(propertyInfo.Name,
                b =>
                {
                    b.RegisterLaw(Law.Axiom(EndpointType.Call));
                    b.RegisterLaw(Law.Axiom(new EndpointMember(propertyInfo)));
                    b.RegisterLaw(Law.Axiom(new MessageType(typeof(TArgs))));
                    b.RegisterLaw(Law.Axiom(new ResponseType(typeof(TResult))));
                });
            return new CallEndpointBuilder<TArgs, TResult>(LoggerFactory, builder);
        }
    }
}