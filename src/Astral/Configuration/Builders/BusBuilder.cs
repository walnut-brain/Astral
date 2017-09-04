using System;
using System.Linq;
using System.Reflection;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Transport;
using Lawium;
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
            var builder = BookBuilder.GetSubBookBuilder(type, b =>
            {
                b.RegisterLaw(Law.Axiom(new ServiceType(type)));
                foreach (var astralAttribute in type.GetCustomAttributes(true).OfType<IAstralAttribute>())
                {
                    var (atype, value) = astralAttribute.GetConfigElement(type.GetTypeInfo());
                    b.RegisterLaw(Law.Axiom(atype, value));
                }
            });
            return new ServiceBuilder<TService>(LoggerFactory, builder);
        }

        public Bus<TTransport> Build<TTransport>(IServiceProvider serviceProvider)
            where TTransport : class, ITransport
        {
            return Build<Bus<TTransport>, TTransport>(serviceProvider, (sp, cfg) => new Bus<TTransport>(sp, cfg));
        }

        public TBus Build<TBus, TTransport>(IServiceProvider serviceProvider,
            Func<IServiceProvider, BusConfig, TBus> busFactory)
            where TBus : Bus<TTransport>
            where TTransport : class, ITransport
        {
            BookBuilder.RegisterLaw(Law.Axiom(new InstanceCode(Guid.NewGuid().ToString("D"))));
            return busFactory(serviceProvider, new BusConfig(BookBuilder.Build()));
        }
    }
}