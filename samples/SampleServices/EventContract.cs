using Astral.Markup;

namespace SampleServices
{
    [Contract("event.contract")]
    public class EventContract
    {
        public string Name { get; set; }
    }
}