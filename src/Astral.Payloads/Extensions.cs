using System;
using System.Linq;
using System.Net.Mime;

namespace Astral.Payloads
{
    public static class Extensions
    {
        public static bool IsJson(this ContentType contentType)
        {
            var types = new[] {"text/json", "application/json"};

            return types.Any(p =>
                string.Compare(contentType.MediaType, p, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}