using System;
using Astral.Transport;

namespace Astral.Deliveries
{
    public abstract class DeliveryReplyTo
    {
        private DeliveryReplyTo()
        {
        }

        public sealed class SystemType : DeliveryReplyTo
        {
            internal SystemType()
            {
            }

            public override ResponseTo ResponseTo => ResponseTo.System;
        }

        public class SubsystemType : DeliveryReplyTo
        {
            internal SubsystemType(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
                Name = name;
            }

            public string Name { get; }

            public override ResponseTo ResponseTo => ResponseTo.Named(Name);
        }
        
        public abstract ResponseTo ResponseTo { get; }
        
        public static readonly DeliveryReplyTo System = new SystemType();
        public static DeliveryReplyTo Subsystem(string name) => new SubsystemType(name);

    }
}