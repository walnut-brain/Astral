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
        public static DeliveryReply WithReply(ChannelKind.IDeliveryReply replyTo)
            => new DeliveryReply(Kind.WithReply, (ChannelKind) replyTo);
        public static DeliveryReply IsReply(ChannelKind.ReplyChannelKind replyChannel) => new DeliveryReply(Kind.IsReply, replyChannel);

        public void Match(Action noReply, Action<ChannelKind.IDeliveryReply> withReply, Action<ChannelKind.ReplyChannelKind> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    noReply();
                    break;
                case Kind.WithReply:
                    withReply((ChannelKind.IDeliveryReply) _replyTo);
                    break;
                case Kind.IsReply:
                    isReply((ChannelKind.ReplyChannelKind) _replyTo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public T Match<T>(Func<T> noReply, Func<ChannelKind.IDeliveryReply, T> withReply, Func<ChannelKind.ReplyChannelKind, T> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    return noReply();
                case Kind.WithReply:
                    return withReply((ChannelKind.IDeliveryReply) _replyTo);
                case Kind.IsReply:
                    return isReply((ChannelKind.ReplyChannelKind) _replyTo);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}