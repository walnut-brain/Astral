using System.Net.Mime;

namespace Astral.Configuration.Settings
{
    public class RpcTransportSelector : Fact<(string, ContentType)>
    {
        public RpcTransportSelector((string, ContentType) value) : base(value)
        {
        }
    }
}