using System;

namespace Astral.Configuration.Settings
{
    
    public delegate TimeSpan MessageToMessageTtl<T>(T message);
}