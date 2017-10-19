using System;
using System.Collections.Generic;
using Astral.Markup;

namespace SampleServices
{
    [Contract("event.contract")]
    public class EventContract
    {
        public string Name { get; set; }
        
        public List<Line> Lines { get; set; }
        
        public class Line
        {
            public Sample Num { get; set; }

            public Code Data { get; set; }
            
            [Flags]
            public enum Code
            {
                Fl1 = 1,
                Fl2 = 2
            }
        }
    }

    public enum Sample
    {
        One = 1, Two = 5
    }
}