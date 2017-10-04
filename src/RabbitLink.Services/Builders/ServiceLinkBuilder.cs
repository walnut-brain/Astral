using System;
using RabbitLink.Builders;
using RabbitLink.Connection;
using RabbitLink.Logging;
using RabbitLink.Serialization;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class ServiceLinkBuilder : IServiceLinkBuilder
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly IDescriptionFactory _descriptionFactory;
        
        private readonly IPayloadManager _payloadManager;
        private readonly string _serviceName;

        public ServiceLinkBuilder(ILinkBuilder linkBuilder, IPayloadManager payloadManager, IDescriptionFactory descriptionFactory, string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceName));
            _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
            _payloadManager = payloadManager ?? throw new ArgumentNullException(nameof(payloadManager));
            _descriptionFactory = descriptionFactory ?? throw new ArgumentNullException(nameof(descriptionFactory));
            _serviceName = serviceName;
        }
        

        public IServiceLinkBuilder ConnectionName(string value)
            => new ServiceLinkBuilder(_linkBuilder.ConnectionName(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder Uri(string value)
            => new ServiceLinkBuilder(_linkBuilder.Uri(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder Uri(Uri value)
            => new ServiceLinkBuilder(_linkBuilder.Uri(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder AutoStart(bool value)
            => new ServiceLinkBuilder(_linkBuilder.AutoStart(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder Timeout(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.Timeout(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder RecoveryInterval(TimeSpan value)
            => new ServiceLinkBuilder(_linkBuilder.RecoveryInterval(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder AppId(string value)
            => new ServiceLinkBuilder(_linkBuilder.AppId(value), _payloadManager, _descriptionFactory, value);

        public IServiceLinkBuilder OnStateChange(LinkStateHandler<LinkConnectionState> handler)
            => new ServiceLinkBuilder(_linkBuilder.OnStateChange(handler), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder UseBackgroundThreadsForConnection(bool value)
            => new ServiceLinkBuilder(_linkBuilder.UseBackgroundThreadsForConnection(value), _payloadManager, _descriptionFactory, _serviceName);


        public IServiceLinkBuilder PayloadManager(Func<IServiceProvider, IPayloadManager> factory)
            => new ServiceLinkBuilder(_linkBuilder, _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLinkBuilder LoggerFactory(ILinkLoggerFactory value)
            => new ServiceLinkBuilder(_linkBuilder.LoggerFactory(value), _payloadManager, _descriptionFactory, _serviceName);

        public IServiceLink Build()
            => new ServiceLink(_linkBuilder.Build(), _payloadManager, _descriptionFactory, _serviceName);
    }
}