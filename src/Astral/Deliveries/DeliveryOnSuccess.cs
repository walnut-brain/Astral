using System;
using System.Threading.Tasks;
using Astral.Data;

namespace Astral.Deliveries
{
    /// <summary>
    /// What to do when delivery was success sended
    /// </summary>
    public abstract class DeliveryOnSuccess
    {
        private DeliveryOnSuccess()
        {
        }

        /// <summary>
        /// Delete alternative
        /// </summary>
        internal class DeleteType : DeliveryOnSuccess
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

        /// <summary>
        /// Archive alternative
        /// </summary>
        internal class ArchiveType : DeliveryOnSuccess
        {
            internal ArchiveType(TimeSpan deleteAfter)
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

        /// <summary>
        /// Redelivery alternative
        /// </summary>
        internal class RedeliveryType : DeliveryOnSuccess
        {
            internal RedeliveryType(TimeSpan redeliveryAfter)
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

        
        
        /// <summary>
        /// Delete delivery after send
        /// </summary>
        public static DeliveryOnSuccess Delete = new DeleteType();
        /// <summary>
        /// Archive delivery in store
        /// </summary>
        /// <param name="deleteAt">when to delete archive</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Archive(DateTimeOffset deleteAt) => new ArchiveType(deleteAt - DateTimeOffset.Now);

        /// <summary>
        /// Archive delivery in store
        /// </summary>
        /// <param name="after">delete after period</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Archive(TimeSpan after) => new ArchiveType(after);

        /// <summary>
        /// Resend delivery
        /// </summary>
        /// <param name="redeliveryAt">send at</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Redelivery(DateTimeOffset redeliveryAt) => new RedeliveryType(redeliveryAt - DateTimeOffset.Now);

        /// <summary>
        /// Resend delivery
        /// </summary>
        /// <param name="after">send after period</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Redelivery(TimeSpan after) => new RedeliveryType(after);
        
    }
}