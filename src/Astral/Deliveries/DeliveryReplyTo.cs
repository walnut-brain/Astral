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
        }

        public class SubsystemType : DeliveryReplyTo
        {
            internal SubsystemType(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
        
        public static readonly DeliveryReplyTo System = new SystemType();
        public static DeliveryReplyTo Subsystem(string name) => new SubsystemType(name);

    }
}