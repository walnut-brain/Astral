using System;

namespace Astral.Transport
{
    public struct SubscribeChannel
    {
        private enum Kind {
            System = 0,
            Named = 1,
            Instance = 2,
            Dedicated = 3,
            Rpc = 4
        }

        private readonly Kind _kind;
        private readonly string _name;

        private SubscribeChannel(Kind kind, string name)
        {
            _kind = kind;
            _name = name;
        }
        
        public static readonly SubscribeChannel System = new SubscribeChannel(Kind.System, null);
        public static readonly SubscribeChannel Instance = new SubscribeChannel(Kind.Instance, null);
        public static readonly SubscribeChannel Dedicated = new SubscribeChannel(Kind.Dedicated, null);
        public static readonly SubscribeChannel Rpc = new SubscribeChannel(Kind.Rpc, null);
        public static SubscribeChannel Named(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new SubscribeChannel(Kind.Named, name);
        }

        public void Match(Action system, Action<string> named, Action instance, Action dedicated, Action rpc)
        {
            switch (_kind)
            {
                case Kind.System:
                    system();
                    break;
                case Kind.Named:
                    named(_name);
                    break;
                case Kind.Instance:
                    instance();
                    break;
                case Kind.Dedicated:
                    dedicated();
                    break;
                case Kind.Rpc:
                    rpc();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public T Match<T>(Func<T> system, Func<string, T> named, Func<T> instance, Func<T> dedicated, Func<T> rpc)
        {
            switch (_kind)
            {
                case Kind.System:
                    return system();
                case Kind.Named:
                    return named(_name);
                case Kind.Instance:
                    return instance();
                case Kind.Dedicated:
                    return dedicated();
                case Kind.Rpc:
                    return rpc();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Equals(SubscribeChannel other)
        {
            return _kind == other._kind && string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SubscribeChannel channel && Equals(channel);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _kind * 397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }
    }
}