using System;
using System.Threading.Tasks;
using Astral.Data;

namespace Astral.Delivery
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

            internal override Task Apply<TStore>(IDeliveryDataService<TStore> service, Guid deliveryId, string sponsor)
            {
                return service
                    .Where(t => t.DeliveryId == deliveryId)
                    .Delete();
            }
        }

        internal class ArchiveType : OnDeliverySuccess
        {
            public ArchiveType(DateTimeOffset deleteAt)
            {
                DeleteAt = deleteAt;
            }

            public DateTimeOffset DeleteAt { get; }

            protected bool Equals(ArchiveType other)
            {
                return DeleteAt.Equals(other.DeleteAt);
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
                return DeleteAt.GetHashCode();
            }

            internal override Task Apply<TStore>(IDeliveryDataService<TStore> service, Guid deliveryId, string sponsor)
            {
                return
                    service
                        .Where(t => t.DeliveryId == deliveryId)
                        .Set(t => t.Delivered, true)
                        .Set(t => t.Ttl, DeleteAt)
                        .Set(t => t.Sponsor, (string) null)
                        .Set(t => t.LeasedTo, DeleteAt)
                        .Update();
            }
        }

        internal class RedeliveryType : OnDeliverySuccess
        {
            public RedeliveryType(DateTimeOffset redeliveryAt)
            {
                RedeliveryAt = redeliveryAt;
            }

            public DateTimeOffset RedeliveryAt { get; }

            protected bool Equals(RedeliveryType other)
            {
                return RedeliveryAt.Equals(other.RedeliveryAt);
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
                return RedeliveryAt.GetHashCode();
            }

            internal override Task Apply<TStore>(IDeliveryDataService<TStore> service, Guid deliveryId, string sponsor)
            {
                return service
                    .Where(t => t.DeliveryId == deliveryId)
                    .Set(t => t.Delivered, false)
                    .Set(t => t.Sponsor, (string) null)
                    .Set(t => t.LeasedTo, RedeliveryAt)
                    .Update();
            }
        }

        internal abstract Task Apply<TStore>(IDeliveryDataService<TStore> service, Guid deliveryId, string sponsor) 
            where TStore : IStore<TStore>;
        

        public static OnDeliverySuccess Delete = new DeleteType();
        public static OnDeliverySuccess Archive(DateTimeOffset deleteAt) => new ArchiveType(deleteAt);
        public static OnDeliverySuccess Archive(TimeSpan after) => new ArchiveType(DateTimeOffset.Now + after);
        public static OnDeliverySuccess Redelivery(DateTimeOffset redeliveryAt) => new RedeliveryType(redeliveryAt);
        public static OnDeliverySuccess Redelivery(TimeSpan after) => new RedeliveryType(DateTimeOffset.Now + after);
        
    }
}