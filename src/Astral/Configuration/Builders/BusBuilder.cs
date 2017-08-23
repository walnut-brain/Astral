﻿using System;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Lawium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class BusBuilder : BuilderBase
    {
        public BusBuilder(string systemName, ILoggerFactory loggerFactory = null) : base(
            loggerFactory, new LawBookBuilder(loggerFactory))
        {
            if (systemName == null) throw new ArgumentNullException(nameof(systemName));
            BookBuilder.RegisterLaw(Law.Axiom(new SystemName(systemName)));
        }

        public ServiceBuilder<TService> Service<TService>()
        {
            var type = typeof(TService);
            if (!type.IsInterface)
                throw new ArgumentException($"{type} must be interface");
            var builder = BookBuilder.GetSubBookBuilder(type, b => b.RegisterLaw(Law.Axiom(new ServiceType(type))));
            return new ServiceBuilder<TService>(LoggerFactory, builder);
        }

        public Bus Build(IServiceProvider serviceProvider)
            => Build(serviceProvider, (sp, cfg) => new Bus(sp, cfg));

        public TBus Build<TBus>(IServiceProvider serviceProvider, Func<IServiceProvider, BusConfig, TBus> busFactory)
            where TBus : Bus
            => busFactory(serviceProvider, new BusConfig(BookBuilder.Build()));

        



    }
}