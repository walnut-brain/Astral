using System;
using Astral.Markup;
using Astral.Markup.RabbitMq;

namespace Contract.Test
{
    [Service("first")]
    [Owner("test")]
    public interface IFirstService
    {
        [Endpoint("event")]
        EventHandler<EventContract> Event { get; }
        
        [Endpoint("test.call")]
        [Exchange(Name = "test.call.exchange", Durable = false)]
        Func<int, int> Call { get; }
        
    }
    
    [SchemaName("EventContract")]
    public class EventContract
    {
        public Line[] Lines { get;set; }
        public string Name { get;set; }
        
        [SchemaName("EventContract.Line")]
        public class Line
        {
            public Code Data { get;set; }
            public Sample Num { get;set; }
            
            [SchemaName("EventContract.Line.Code")]
            [Flags]
            public enum Code
            {
                Fl1 = 1,
                Fl2 = 2
            }
        }
    }
    
    [SchemaName("Sample")]
    public enum Sample
    {
        Two = 5,
        One = 1
    }
}
