using System;
using Astral.Configuration.Settings;
using Astral.Lawium;

namespace Astral.Configuration.Configs
{
    public class BusConfig : ConfigBase
    {
        internal BusConfig(LawBook lawBook) : base(lawBook)
        {
        }

        public ServiceConfig<TService> Service<TService>()
        {
            var type = typeof(TService);
            if(!type.IsInterface)
                throw new ArgumentException($"{type} must be interface");
            var book = LawBook.GetOrAddSubBook(type, b =>
            {
                b.RegisterLaw(Law.Axiom(new ServiceType(type)));
            }).Result;
            return new ServiceConfig<TService>(book);
        }

        public ServiceConfig Service(Type serviceType)
        {
            if (!serviceType.IsInterface)
                throw new ArgumentException($"{serviceType} must be interface");
            var book = LawBook.GetOrAddSubBook(serviceType, b =>
            {
                b.RegisterLaw(Law.Axiom(new ServiceType(serviceType)));
            }).Result;
            var cfgType = typeof(ServiceConfig<>).MakeGenericType(serviceType);
            return (ServiceConfig) Activator.CreateInstance(cfgType, book);

        }

    }
}