using System;

namespace Astral
{
    /// <summary>
    /// Channel for listen 
    /// </summary>
    public abstract class ChannelKind
    {
        private ChannelKind()
        {
        }
        
        public interface IEventChannel
        {
            
        }
        
        public interface IDeliveryReply : IResponseTo
        {
            
        }
        
        public interface IResponseTo
        {
            
        }

        /// <summary>
        /// System channel - channel common to all subscribers by System Name
        /// </summary>
        public static readonly SystemChannelKind System = new SystemChannelKind();
        
        /// <summary>
        /// Instance channel - channel specific for runned instance of bus
        /// </summary>
        public static readonly InstanceChannelKind Instance = new InstanceChannelKind();
        
        /// <summary>
        /// Dedicated channel - channel per subscription  
        /// </summary>
        public static readonly DedicatedChannelKind Dedicated = new DedicatedChannelKind();
        
        /// <summary>
        /// Rpc channel - channel specificaly designed to optimize rpc calls
        /// </summary>
        public static readonly RpcChannelKind Rpc = new RpcChannelKind();
        
        /// <summary>
        /// Named channel - channel with specific name
        /// </summary>
        /// <param name="name">name of channel</param>
        /// <returns></returns>
        public static NamedChannelKind Named(string name) => new NamedChannelKind(name);
        
        /// <summary>
        /// Reply channel - channel specific to request
        /// </summary>
        /// <param name="replyTo">transport specific channel specification</param>
        /// <param name="requestId">request correlation id</param>
        /// <returns></returns>
        public static ReplyChannelKind Reply(string replyTo, string requestId) => new ReplyChannelKind(replyTo, requestId);
        
        /// <summary>
        /// None channel
        /// </summary>
        public static readonly NoneChannelKind None = new NoneChannelKind();
            
        public void Match(Action system, Action<string> named, Action instance, Action dedicated, Action rpc, Action<string, string> reply,
            Action none)
        {
            switch (this)
            {
                case DedicatedChannelKind _:
                    dedicated();
                    break;
                case InstanceChannelKind _:
                    instance();
                    break;
                case NamedChannelKind namedChannel:
                    named(namedChannel.Name);
                    break;
                case NoneChannelKind _:
                    none();
                    break;
                case ReplyChannelKind replyChannel:
                    reply(replyChannel.ReplyTo, replyChannel.RequestId);
                    break;
                case RpcChannelKind _:
                    rpc();
                    break;
                case SystemChannelKind _:
                    system();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public T Match<T>(Func<T> system, Func<string, T> named, Func<T> instance, Func<T> dedicated, Func<T> rpc, Func<string, string, T> reply,
            Func<T> none)
        {
            switch (this)
            {
                case DedicatedChannelKind _:
                    return dedicated();
                case InstanceChannelKind _:
                    return instance();
                case NamedChannelKind namedChannel:
                    return named(namedChannel.Name);
                case NoneChannelKind _:
                    return none();
                case ReplyChannelKind replyChannel:
                    return reply(replyChannel.ReplyTo, replyChannel.RequestId);                    
                case RpcChannelKind _:
                    return rpc();
                case SystemChannelKind _:
                    return system();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } 
        
        /// <summary>
        /// System channel type
        /// </summary>
        public sealed class SystemChannelKind : ChannelKind, IEventChannel, IDeliveryReply, IResponseTo
        {
            internal SystemChannelKind()
            {
            }

            private bool Equals(SystemChannelKind other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SystemChannelKind channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// System channel type
        /// </summary>
        public sealed class NoneChannelKind : ChannelKind, IResponseTo
        {
            internal NoneChannelKind()
            {
            }

            private bool Equals(NoneChannelKind other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is NoneChannelKind channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Instance channel type
        /// </summary>
        public sealed class InstanceChannelKind : ChannelKind, IEventChannel, IResponseTo
        {
            internal InstanceChannelKind()
            {
            }

            private bool Equals(InstanceChannelKind other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is InstanceChannelKind channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Dedicated channel type
        /// </summary>
        public sealed class DedicatedChannelKind : ChannelKind, IEventChannel
        {
            internal DedicatedChannelKind()
            {
            }

            private bool Equals(DedicatedChannelKind other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DedicatedChannelKind channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Rpc channel type
        /// </summary>
        public sealed class RpcChannelKind : ChannelKind, IResponseTo
        {
            internal RpcChannelKind()
            {
            }

            private bool Equals(RpcChannelKind other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is RpcChannelKind channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Named channel type
        /// </summary>
        public sealed class NamedChannelKind : ChannelKind, IEventChannel, IDeliveryReply, IResponseTo
        {
            internal NamedChannelKind(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
                Name = name;
            }

            public string Name { get; }

            private bool Equals(NamedChannelKind other) => string.Equals(Name, other.Name);

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is NamedChannelKind channel && Equals(channel);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (base.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                }
            }
        }
        
        
        /// <summary>
        /// Direct channel type
        /// </summary>
        public sealed class ReplyChannelKind : ChannelKind, IResponseTo
        {
            public string ReplyTo { get; }
            public string RequestId { get; }

            internal ReplyChannelKind(string replyTo, string requestId)
            {
                ReplyTo = replyTo;
                RequestId = requestId;
            }

            private bool Equals(ReplyChannelKind other)
            {
                return string.Equals(ReplyTo, other.ReplyTo) && string.Equals(RequestId, other.RequestId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ReplyChannelKind && Equals((ReplyChannelKind) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ReplyTo != null ? ReplyTo.GetHashCode() : 0) * 397) ^ (RequestId != null ? RequestId.GetHashCode() : 0);
                }
            }
        }
        
        
            
        
        
        
        
        

        

        
    }
}