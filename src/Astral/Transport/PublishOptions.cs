using System;

namespace Astral.Transport
{
    public class PublishOptions
    {
        public PublishOptions(TimeSpan messageTtl)
        {
            MessageTtl = messageTtl;
        }

        public TimeSpan MessageTtl { get; }
    }
}