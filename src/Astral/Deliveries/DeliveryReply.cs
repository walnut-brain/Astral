using System;
using FunEx.Monads;

namespace Astral.Deliveries
{
    public struct DeliveryReply
    {
        private enum Kind
        {
            NoReply = 0,
            WithReply = 1,
            IsReply = 2
        }

        private readonly Kind _kind;
        private readonly ChannelKind _replyTo;


        private DeliveryReply(Kind kind, ChannelKind replyTo)
        {
            _kind = kind;
            _replyTo = replyTo;
        }


        public static readonly DeliveryReply NoReply = default(DeliveryReply);
        public static DeliveryReply WithReply(ChannelKind.DurableChannel replyTo)
            => new DeliveryReply(Kind.WithReply, replyTo);
        public static DeliveryReply IsReply(ChannelKind.ReplyChannel replyChannel) => new DeliveryReply(Kind.IsReply, replyChannel);

        public void Match(Action noReply, Action<ChannelKind.DurableChannel> withReply, Action<ChannelKind.ReplyChannel> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    noReply();
                    break;
                case Kind.WithReply:
                    withReply((ChannelKind.DurableChannel) _replyTo);
                    break;
                case Kind.IsReply:
                    isReply((ChannelKind.ReplyChannel) _replyTo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public T Match<T>(Func<T> noReply, Func<ChannelKind.DurableChannel, T> withReply, Func<ChannelKind.ReplyChannel, T> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    return noReply();
                case Kind.WithReply:
                    return withReply((ChannelKind.DurableChannel) _replyTo);
                case Kind.IsReply:
                    return isReply((ChannelKind.ReplyChannel) _replyTo);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}