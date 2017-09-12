using System;
using Astral.Transport;

namespace Astral.Deliveries
{
    public struct DeliveryReplyTo
    {
        private readonly bool _toSubsystem;
        private readonly string _subSystem;

        private DeliveryReplyTo(string subSystem)
        {
            _toSubsystem = !string.IsNullOrWhiteSpace(subSystem);
            _subSystem = subSystem;
        }



        public ResponseTo ResponseTo => _toSubsystem ? ResponseTo.Named(_subSystem) : ResponseTo.System; 
        
        public static readonly DeliveryReplyTo System = default(DeliveryReplyTo);
        public static DeliveryReplyTo Subsystem(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return new DeliveryReplyTo(name);
        }

        public void Match(Action onSystem, Action<string> onSubsystem)
        {
            if (_toSubsystem)
                onSubsystem(_subSystem);
            else
                onSystem();  
        }

        public T Match<T>(Func<T> onSystem, Func<string, T> onSubsystem)
            => _toSubsystem ? onSubsystem(_subSystem) : onSystem();
    }
}