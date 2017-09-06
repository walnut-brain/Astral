namespace Astral.Deliveries
{
    /// <summary>
    /// What to do after success commit delivery to store
    /// </summary>
    public abstract class DeliveryAfterCommit
    {
        private DeliveryAfterCommit()
        {
        }

        /// <summary>
        /// NoOp alternative
        /// </summary>
        public sealed class NoOpType : DeliveryAfterCommit
        {
            
        }

        /// <summary>
        /// Send alternative
        /// </summary>
        public sealed class SendType : DeliveryAfterCommit
        {
            public SendType(DeliveryOnSuccess deliveryOnSuccess)
            {
                DeliveryOnSuccess = deliveryOnSuccess;
            }

            public DeliveryOnSuccess DeliveryOnSuccess { get; }
        }

        /// <summary>
        /// Nothing. Delivery will be maked by other service
        /// </summary>
        public static DeliveryAfterCommit NoOp = new NoOpType();

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="deliveryOnSuccess">What to do when delivery was sended</param>
        /// <returns>send alternative</returns>
        public static DeliveryAfterCommit Send(DeliveryOnSuccess deliveryOnSuccess) => new SendType(deliveryOnSuccess);
    }
}