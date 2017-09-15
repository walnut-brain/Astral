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

        /// <summary>
        /// System channel - channel common to all subscribers by System Name
        /// </summary>
        public static readonly SystemChannel System = new SystemChannel();
        
        /// <summary>
        /// Instance channel - channel specific for runned instance of bus
        /// </summary>
        public static readonly InstanceChannel Instance = new InstanceChannel();
        
        /// <summary>
        /// Dedicated channel - channel per subscription  
        /// </summary>
        public static readonly DedicatedChannel Dedicated = new DedicatedChannel();
        
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
        public static ReplyChannel Reply(string replyTo, string requestId) => new ReplyChannel(replyTo, requestId);
        
        /// <summary>
        /// None channel
        /// </summary>
        public static readonly NoneChannel None = new NoneChannel();
            
        public abstract class RespondableChannel : ChannelKind
        {
            internal RespondableChannel()
            {
            }
        }
        
        public abstract class DurableChannel : RespondableChannel
        {
            internal DurableChannel()
            {
            }
        }
        
        /// <summary>
        /// System channel type
        /// </summary>
        public sealed class SystemChannel : DurableChannel, IEventChannel
        {
            internal SystemChannel()
            {
            }

            private bool Equals(SystemChannel other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SystemChannel channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }


        
        
        /// <summary>
        /// System channel type
        /// </summary>
        public sealed class NoneChannel : RespondableChannel
        {
            internal NoneChannel()
            {
            }

            private bool Equals(NoneChannel other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is NoneChannel channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Instance channel type
        /// </summary>
        public sealed class InstanceChannel : ChannelKind, IEventChannel
        {
            internal InstanceChannel()
            {
            }

            private bool Equals(InstanceChannel other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is InstanceChannel channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Dedicated channel type
        /// </summary>
        public sealed class DedicatedChannel : ChannelKind, IEventChannel
        {
            internal DedicatedChannel()
            {
            }

            private bool Equals(DedicatedChannel other) => true;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DedicatedChannel channel && Equals(channel);
            }

            public override int GetHashCode() => GetType().GetHashCode();
        }
        
        /// <summary>
        /// Rpc channel type
        /// </summary>
        public sealed class RpcChannelKind : RespondableChannel
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
        public sealed class NamedChannelKind : DurableChannel, IEventChannel
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
        public sealed class ReplyChannel : RespondableChannel
        {
            public string ReplyTo { get; }
            public string RequestId { get; }

            internal ReplyChannel(string replyTo, string requestId)
            {
                ReplyTo = replyTo;
                RequestId = requestId;
            }

            private bool Equals(ReplyChannel other)
            {
                return string.Equals(ReplyTo, other.ReplyTo) && string.Equals(RequestId, other.RequestId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ReplyChannel && Equals((ReplyChannel) obj);
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