using System;
using Astral.Specifications;

namespace Astral.Transport
{
    public interface ITransport 
    {
        PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, bool isReply, ChannelKind responseTo);
        
        (string, Subscribable) GetChannel(ChannelConfig config);
    }
}