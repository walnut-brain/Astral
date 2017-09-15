using System;
using Astral.Configuration.Settings;
using Astral.Transport;
using FunEx.Monads;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Specifications
{
    public class ChannelConfig : IServiceProvider
    {
        private readonly LawBook _lawBook;
        public EndpointConfig Endpoint { get; }

        internal ChannelConfig(LawBook lawBook, EndpointConfig endpoint)
        {
            _lawBook = lawBook;
            Endpoint = endpoint;
        }

        public object GetService(Type serviceType)
            => _lawBook.TryGet(serviceType).OfType<object>().OrElse(() => Endpoint.GetService(serviceType).ToOption()).IfNoneDefault();

        public ChannelKind ChannelKind => this.GetRequiredService<SubscribeChannelSetting>().Value;
        public bool IsResponse => this.GetRequiredService<IsResponseChannelSetting>().Value;
    }
}