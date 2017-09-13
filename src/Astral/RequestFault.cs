namespace Astral
{
    [Contract("fault")]
    public class RequestFault
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}