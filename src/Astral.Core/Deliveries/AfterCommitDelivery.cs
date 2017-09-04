namespace Astral.Deliveries
{
    public abstract class AfterCommitDelivery
    {
        private AfterCommitDelivery()
        {
        }

        public sealed class NoOpType : AfterCommitDelivery
        {
            
        }

        public sealed class SendType : AfterCommitDelivery
        {
            public SendType(OnDeliverySuccess onSuccess)
            {
                OnSuccess = onSuccess;
            }

            public OnDeliverySuccess OnSuccess { get; }
        }

        public static AfterCommitDelivery NoOp = new NoOpType();
        public static AfterCommitDelivery Send(OnDeliverySuccess onSuccess) => new SendType(onSuccess);
    }
}