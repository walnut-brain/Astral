using System;
using System.Threading.Tasks;
using Astral.Data;

namespace Astral.Deliveries
{
    public abstract class OnDeliverySuccess
    {
        private OnDeliverySuccess()
        {
        }

        internal class DeleteType : OnDeliverySuccess
        {
            protected bool Equals(DeleteType other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DeleteType) obj);
            }

            public override int GetHashCode() => typeof(DeleteType).GetHashCode();

        }

        internal class ArchiveType : OnDeliverySuccess
        {
            public ArchiveType(TimeSpan deleteAfter)
            {
                DeleteAfter = deleteAfter;
            }

            public TimeSpan DeleteAfter { get; }

            protected bool Equals(ArchiveType other)
            {
                return DeleteAfter.Equals(other.DeleteAfter);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ArchiveType) obj);
            }

            public override int GetHashCode()
            {
                return DeleteAfter.GetHashCode();
            }
            
        }

        internal class RedeliveryType : OnDeliverySuccess
        {
            public RedeliveryType(TimeSpan redeliveryAfter)
            {
                RedeliveryAfter = redeliveryAfter;
            }

            public TimeSpan RedeliveryAfter { get; }

            protected bool Equals(RedeliveryType other)
            {
                return RedeliveryAfter.Equals(other.RedeliveryAfter);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((RedeliveryType) obj);
            }

            public override int GetHashCode()
            {
                return RedeliveryAfter.GetHashCode();
            }

        }

        
        

        public static OnDeliverySuccess Delete = new DeleteType();
        public static OnDeliverySuccess Archive(DateTimeOffset deleteAt) => new ArchiveType(deleteAt - DateTimeOffset.Now);
        public static OnDeliverySuccess Archive(TimeSpan after) => new ArchiveType(after);
        public static OnDeliverySuccess Redelivery(DateTimeOffset redeliveryAt) => new RedeliveryType(redeliveryAt - DateTimeOffset.Now);
        public static OnDeliverySuccess Redelivery(TimeSpan after) => new RedeliveryType(after);
        
    }
}