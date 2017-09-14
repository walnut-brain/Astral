using System;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface ITransport 
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, ChannelKind responseTo);
        
        (string, Subscribable) GetChannel(ChannelConfig config);
    }
}