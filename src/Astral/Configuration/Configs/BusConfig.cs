using System;
using System.Linq;
using System.Reflection;
using Astral.Configuration.Settings;
using Lawium;

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
            if (!type.IsInterface)
                throw new ArgumentException($"{type} must be interface");
            var book = LawBook.GetOrAddSubBook(type, b =>
            {
                b.RegisterLaw(Law.Axiom(new ServiceType(type)));
                foreach (var astralAttribute in type.GetCustomAttributes(true).OfType<IAstralAttribute>())
                {
                    var (atype, value) = astralAttribute.GetConfigElement(type.GetTypeInfo());
                    b.RegisterLaw(Law.Axiom(atype, value));
                }
            }).Result;
            return new ServiceConfig<TService>(book);
        }

        public ServiceConfig Service(Type serviceType)
        {
            if (!serviceType.IsInterface)
                throw new ArgumentException($"{serviceType} must be interface");
            var book = LawBook
                .GetOrAddSubBook(serviceType, b =>
                {
                    b.RegisterLaw(Law.Axiom(new ServiceType(serviceType)));
                    foreach (var astralAttribute in serviceType.GetCustomAttributes(true).OfType<IAstralAttribute>())
                    {
                        var (atype, value) = astralAttribute.GetConfigElement(serviceType.GetTypeInfo());
                        b.RegisterLaw(Law.Axiom(atype, value));
                    }
                }).Result;
            var cfgType = typeof(ServiceConfig<>).MakeGenericType(serviceType);
            return (ServiceConfig) Activator.CreateInstance(cfgType, book);
        }
    }
}