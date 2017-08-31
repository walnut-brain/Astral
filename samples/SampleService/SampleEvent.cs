using Astral.Markup;

namespace SampleService
{
    [Contract("sample.event")]
    public class SampleEvent
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}