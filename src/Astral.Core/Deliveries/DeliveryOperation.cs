using System;
using System.Security.Permissions;
using Astral.Contracts;

namespace Astral.Deliveries
{
    public abstract class DeliveryOperation
    {
        public SystemName System { get; }
        public TransportTag Transport { get; }
        public OperationName Operation { get; }

        public class Send : DeliveryOperation
        {
            
        }

        public class SendWithReplay : DeliveryOperation
        {
            public DeliveryReplayTo ReplayTo { get; }
        }

        public class Replay : DeliveryOperation
        {
            public DeliveryReplayTo ReplayTo { get; }    
            public string RequestCorrelationId { get; }
        }
    }

    public abstract class DeliveryReplayTo
    {
        public class System : DeliveryReplayTo
        {
            
        }

        public class Subsystem : DeliveryReplayTo
        {
            
        }
    }
}