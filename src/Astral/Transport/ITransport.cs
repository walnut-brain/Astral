using System;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface ITransport 
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, PublishOptions options);
        
        (string, Subscribable) GetChannel(ChannelConfig config);
    }
}