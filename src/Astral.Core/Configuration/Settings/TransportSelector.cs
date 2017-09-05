using System.Net.Mime;

namespace Astral.Configuration.Settings
{
    public class TransportSelector : Fact<(string, ContentType)>
    {
        public TransportSelector((string, ContentType) value) : base(value)
        {
        }
    }
}