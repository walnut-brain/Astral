using Astral.Core;

namespace SampleService
{
    [Service("1.0", "sample.service")]
    public interface ISampleService
    {
        [Endpoint("awesome.event")]
        IEvent<SampleEvent> AwesomeEvent { get; }
    }
}