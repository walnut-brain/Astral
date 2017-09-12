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
        private readonly Option<DeliveryReplyTo> _replyTo;
        private readonly string _replyToEncoded;
        private readonly string _replyOn;

        private DeliveryReply(DeliveryReplyTo replyTo)
        {
            _kind = Kind.WithReply;
            _replyTo = replyTo;
            _replyToEncoded = null;
            _replyOn = null;
        }

        private DeliveryReply(string replyTo, string replyOn)
        {
            if (string.IsNullOrWhiteSpace(replyTo))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(replyTo));
            if (string.IsNullOrWhiteSpace(replyOn))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(replyOn));
            _kind = Kind.IsReply;
            _replyTo = Option.None;
            _replyToEncoded = replyTo;
            _replyOn = replyOn;
        }

        public static readonly DeliveryReply NoReply = default(DeliveryReply);
        public static DeliveryReply WithReply(DeliveryReplyTo replyTo) => new DeliveryReply(replyTo);
        public static DeliveryReply IsReply(string replyTo, string replyOn) => new DeliveryReply(replyTo, replyOn);

        public void Match(Action noReply, Action<DeliveryReplyTo> withReply, Action<string, string> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    noReply();
                    break;
                case Kind.WithReply:
                    withReply(_replyTo.Unwrap());
                    break;
                case Kind.IsReply:
                    isReply(_replyToEncoded, _replyOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public T Match<T>(Func<T> noReply, Func<DeliveryReplyTo, T> withReply, Func<string, string, T> isReply)
        {
            switch (_kind)
            {
                case Kind.NoReply:
                    return noReply();
                case Kind.WithReply:
                    return withReply(_replyTo.Unwrap());
                case Kind.IsReply:
                    return isReply(_replyToEncoded, _replyOn);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}