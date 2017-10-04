using System;
using RabbitLink.Builders;
using RabbitLink.Connection;

namespace RabbitLink.Services
{
    public interface IServiceLinkBuilder 
    {
        IServiceLinkBuilder ConnectionName(string value);
        IServiceLinkBuilder Uri(string value);
        IServiceLinkBuilder Uri(Uri value);
        IServiceLinkBuilder AutoStart(bool value);
        IServiceLinkBuilder Timeout(TimeSpan value);
        IServiceLinkBuilder RecoveryInterval(TimeSpan value);
        IServiceLinkBuilder AppId(string value);
        IServiceLinkBuilder OnStateChange(LinkStateHandler<LinkConnectionState> handler);
        IServiceLinkBuilder UseBackgroundThreadsForConnection(bool value);
        IServiceLinkBuilder PayloadManager(Func<IServiceProvider, IPayloadManager> factory);

        IServiceLink Build();
    }
}