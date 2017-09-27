using Astral;

namespace SampleService
{
    [Service("sample.service")]
    [Version("1.0")]
    public interface ISampleService
    {
        [Endpoint("awesome.event")]
        IEvent<SampleEvent> AwesomeEvent { get; }
        
        [Endpoint("sample.event")]
        ICall<SampleEvent, int> Command { get; }
    }
}