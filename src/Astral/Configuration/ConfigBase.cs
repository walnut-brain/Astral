using System;
using Astral.Configuration.Settings;
using FunEx.Monads;
using Lawium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Specifications
{
    public class ConfigBase : IServiceProvider
    {
        protected IServiceProvider Provider { get; }

        public ConfigBase(LawBook<Fact> lawBook, IServiceProvider provider)
        {
            Provider = provider;
            LawBook = lawBook;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        protected LawBook<Fact> LawBook { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public virtual object GetService(Type serviceType)
        {
            return LawBook.TryGet(serviceType).OfType<object>()
                .OrElse(() => Provider.GetService(serviceType).ToOption())
                .IfNoneDefault();
        } 

        public string SystemName => this.GetRequiredService<SystemNameSetting>().Value;
        public string InstanceCode => this.GetRequiredService<InstanceCodeSetting>().Value;

    }
}