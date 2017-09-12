using System;
using System.Threading.Tasks;
using Astral.Data;

namespace Astral.Deliveries
{
    /// <summary>
    /// What to do when delivery was success sended
    /// </summary>
    public struct DeliveryOnSuccess
    {
        private readonly bool _archive;
        private readonly TimeSpan? _redeliver;

        private DeliveryOnSuccess(bool archive) : this()
        {
            _archive = archive;
            _redeliver = null;
        }

        private DeliveryOnSuccess(TimeSpan redeliver) : this()
        {
            _redeliver = redeliver;
            _archive = false;
        }


        /// <summary>
        /// Delete delivery after send
        /// </summary>
        public static DeliveryOnSuccess Delete = default(DeliveryOnSuccess);

        /// <summary>
        /// Archive delivery in store
        /// </summary>

        /// <returns></returns>
        public static readonly DeliveryOnSuccess Archive = new DeliveryOnSuccess(true);


        /// <summary>
        /// Resend delivery
        /// </summary>
        /// <param name="redeliveryAt">send at</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Retry(DateTimeOffset redeliveryAt) =>
            new DeliveryOnSuccess(redeliveryAt - DateTimeOffset.Now);

        /// <summary>
        /// Resend delivery
        /// </summary>
        /// <param name="after">send after period</param>
        /// <returns></returns>
        public static DeliveryOnSuccess Retry(TimeSpan after) => new DeliveryOnSuccess(after);

        public void Match(Action onDelete, Action onArchive, Action<TimeSpan> onRetry)
        {
            if (_redeliver == null)
            {
                if (_archive) onArchive();
                else onDelete();
            }
            else
                onRetry(_redeliver.Value);
        }

        public T Match<T>(Func<T> onDelete, Func<T> onArchive, Func<TimeSpan, T> onRetry)
        {
            if (_redeliver == null)
            {
                return _archive ? onArchive() : onDelete();
            }
            return onRetry(_redeliver.Value);
        }
    }
}