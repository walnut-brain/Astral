using System;

namespace Astral.Delivery
{
    public interface IDeliveryCloseOperations
    {
        void Delete();
        void SetException(Exception exception);
        void Archive(TimeSpan archiveTime);
    }
}