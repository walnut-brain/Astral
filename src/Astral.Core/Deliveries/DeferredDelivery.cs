using System;

namespace Astral.Deliveries
{
    public class DeferredDelivery 
    {                                                                                                                                                                                                                                                                                                                                              
        public Action Commit { get; }
        public Action Rollback { get; }
        
        
    }
}