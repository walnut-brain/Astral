using System;
using System.Collections.Generic;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.RabbitLink.Logging;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Builders;
using RabbitLink.Connection;

namespace Astral.RabbitLink
{
    public class ServiceLinkBuilder : BuilderBase, IServiceLinkBuilder
    {
        private readonly ILinkBuilder _linkBuilder;
        
        
        public ServiceLinkBuilder() : base(new Dictionary<string, object>
        {
            { nameof(DescriptionFactory), new DefaultDescriptionFactory() },
            { nameof(PayloadManager), new DefaultPayloadManager()}
        })
        {
            _linkBuilder = 
                LinkBuilder.Configure
                .AutoStart(AutoStart())
                .Timeout(Timeout())
                .RecoveryInterval(RecoveryInterval())
                .UseBackgroundThreadsForConnection(UseBackgroundThreadsForConnection());
            
        }

        private ServiceLinkBuilder(ILinkBuilder linkBuilder, IReadOnlyDictionary<string, object> store) : base(store)
        {
            _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        }


        public string ConnectionName() => GetParameter(nameof(ConnectionName), (string) null);
        public IServiceLinkBuilder ConnectionName(string value)
            => new ServiceLinkBuilder(_linkBuilder.ConnectionName(value), SetParameter(nameof(ConnectionName), value));

        
        public IServiceLinkBuilder Uri(string value)
            => Uri(value == null ? null : new Uri(value));

        public Uri Uri() => GetParameter(nameof(Uri), (Uri) null);
        public IServiceLinkBuilder Uri(Uri value)
            => new ServiceLinkBuilder(_linkBuilder.Uri(value), SetParameter(nameof(Uri), value));

        public bool AutoStart() => GetParameter(nameof(AutoStart), true);
        public IServiceLinkBuilder AutoStart(bool value)
            => new ServiceLinkBuilder(_linkBuilder.AutoStart(value), SetParameter(nameof(AutoStart), value));


        public TimeSpan Timeout() => GetParameter(nameof(Timeout), TimeSpan.FromSeconds(10));
        public IServiceLinkBuilder Timeout(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.Timeout(value), SetParameter(nameof(Timeout), value));

        public TimeSpan RecoveryInterval() => GetParameter(nameof(RecoveryInterval), TimeSpan.FromSeconds(10));
        public IServiceLinkBuilder RecoveryInterval(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.RecoveryInterval(value), SetParameter(nameof(RecoveryInterval), value));

        public string HolderName() => GetParameter(nameof(HolderName), (string) null);
        public IServiceLinkBuilder HolderName(string value)
            => new ServiceLinkBuilder(_linkBuilder.AppId(value), SetParameter(nameof(HolderName), value));

        public LinkStateHandler<LinkConnectionState> OnStateChange() =>
            GetParameter(nameof(OnStateChange), (LinkStateHandler<LinkConnectionState>) null);
        public IServiceLinkBuilder OnStateChange(LinkStateHandler<LinkConnectionState> handler)
            => new ServiceLinkBuilder(_linkBuilder.OnStateChange(handler), SetParameter(nameof(OnStateChange), handler));

        public bool UseBackgroundThreadsForConnection() => GetParameter(nameof(UseBackgroundThreadsForConnection), false);
        public IServiceLinkBuilder UseBackgroundThreadsForConnection(bool value)
            => new ServiceLinkBuilder(_linkBuilder.UseBackgroundThreadsForConnection(value),
                SetParameter(nameof(UseBackgroundThreadsForConnection), value));


        public IPayloadManager PayloadManager() => GetParameter(nameof(PayloadManager), (IPayloadManager) null);
        public IServiceLinkBuilder PayloadManager(IPayloadManager value)
            => new ServiceLinkBuilder(_linkBuilder, SetParameter(nameof(PayloadManager), value));

        public IDescriptionFactory DescriptionFactory() =>
            GetParameter(nameof(DescriptionFactory), (IDescriptionFactory) null);
        public IServiceLinkBuilder DescriptionFactory(IDescriptionFactory value)
            => new ServiceLinkBuilder(_linkBuilder, SetParameter(nameof(DescriptionFactory), value));
            

        public ILoggerFactory LoggerFactory() => GetParameter(nameof(LoggerFactory), (ILoggerFactory) null);

        public IServiceLinkBuilder LoggerFactory(ILoggerFactory value)
            => new ServiceLinkBuilder(_linkBuilder,
                SetParameter(nameof(LoggerFactory), value));

        public IServiceLink Build()
        {
            var loggerFactory = LoggerFactory() ?? new FakeLoggerFactory();
            _linkBuilder.LoggerFactory(new LoggerFactoryAdapter(loggerFactory));
            return new ServiceLink(_linkBuilder.Build(), PayloadManager(), DescriptionFactory(), HolderName(),
                loggerFactory);
        }
            
    }
}