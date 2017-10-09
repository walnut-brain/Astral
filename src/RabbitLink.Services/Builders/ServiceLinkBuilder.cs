using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RabbitLink.Builders;
using RabbitLink.Connection;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Internals;
using RabbitLink.Services.Logging;

namespace RabbitLink.Services
{
    public class ServiceLinkBuilder : BuilderBase, IServiceLinkBuilder
    {
        private readonly ILinkBuilder _linkBuilder;
        
        
        
        public ServiceLinkBuilder()
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


        public string ConnectionName() => GetValue(nameof(ConnectionName), (string) null);
        public IServiceLinkBuilder ConnectionName(string value)
            => new ServiceLinkBuilder(_linkBuilder.ConnectionName(value), SetValue(nameof(ConnectionName), value));

        
        public IServiceLinkBuilder Uri(string value)
            => Uri(value == null ? null : new Uri(value));

        public Uri Uri() => GetValue(nameof(Uri), (Uri) null);
        public IServiceLinkBuilder Uri(Uri value)
            => new ServiceLinkBuilder(_linkBuilder.Uri(value), SetValue(nameof(Uri), value));

        public bool AutoStart() => GetValue(nameof(AutoStart), true);
        public IServiceLinkBuilder AutoStart(bool value)
            => new ServiceLinkBuilder(_linkBuilder.AutoStart(value), SetValue(nameof(AutoStart), value));


        public TimeSpan Timeout() => GetValue(nameof(Timeout), TimeSpan.FromSeconds(10));
        public IServiceLinkBuilder Timeout(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.Timeout(value), SetValue(nameof(Timeout), value));

        public TimeSpan RecoveryInterval() => GetValue(nameof(RecoveryInterval), TimeSpan.FromSeconds(10));
        public IServiceLinkBuilder RecoveryInterval(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.RecoveryInterval(value), SetValue(nameof(RecoveryInterval), value));

        public string HolderName() => GetValue(nameof(HolderName), (string) null);
        public IServiceLinkBuilder HolderName(string value)
            => new ServiceLinkBuilder(_linkBuilder.AppId(value), SetValue(nameof(HolderName), value));

        public LinkStateHandler<LinkConnectionState> OnStateChange() =>
            GetValue(nameof(OnStateChange), (LinkStateHandler<LinkConnectionState>) null);
        public IServiceLinkBuilder OnStateChange(LinkStateHandler<LinkConnectionState> handler)
            => new ServiceLinkBuilder(_linkBuilder.OnStateChange(handler), SetValue(nameof(OnStateChange), handler));

        public bool UseBackgroundThreadsForConnection() => GetValue(nameof(UseBackgroundThreadsForConnection), false);
        public IServiceLinkBuilder UseBackgroundThreadsForConnection(bool value)
            => new ServiceLinkBuilder(_linkBuilder.UseBackgroundThreadsForConnection(value),
                SetValue(nameof(UseBackgroundThreadsForConnection), value));


        public IPayloadManager PayloadManager() => GetValue(nameof(PayloadManager), (IPayloadManager) null);
        public IServiceLinkBuilder PayloadManager(IPayloadManager value)
            => new ServiceLinkBuilder(_linkBuilder, SetValue(nameof(PayloadManager), value));

        public IDescriptionFactory DescriptionFactory() =>
            GetValue(nameof(DescriptionFactory), (IDescriptionFactory) null);
        public IServiceLinkBuilder DescriptionFactory(IDescriptionFactory value)
            => new ServiceLinkBuilder(_linkBuilder, SetValue(nameof(DescriptionFactory), value));
            

        public ILoggerFactory LoggerFactory() => GetValue(nameof(LoggerFactory), (ILoggerFactory) null);

        public IServiceLinkBuilder LoggerFactory(ILoggerFactory value)
            => new ServiceLinkBuilder(_linkBuilder,
                SetValue(nameof(LoggerFactory), value));

        public IServiceLink Build()
        {
            var loggerFactory = LoggerFactory() ?? new FakeLoggerFactory();
            _linkBuilder.LoggerFactory(new LoggerFactoryAdapter(loggerFactory));
            return new ServiceLink(_linkBuilder.Build(), PayloadManager(), DescriptionFactory(), HolderName(),
                loggerFactory);
        }
            
    }
}