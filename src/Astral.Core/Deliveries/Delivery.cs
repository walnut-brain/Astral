using System;
using Astral.Contracts;

namespace Astral.Deliveries
{
    public static class Delivery
    {
        public static DeferredDelivery Prepare<T>(OperationName operation, )
        {
            throw new NotImplementedException();
        }
    }

    public class DeferredDelivery 
    {                                                                                                                                                                                                                                                                                                                                              
        public Action Commit { get; }
        public Action Rollback { get; }
        
        
    }
}