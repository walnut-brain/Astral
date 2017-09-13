using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    public class ServiceSchema
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Title { get; set; }
        public Dictionary<string, EventSchema> Events { get; set; } = new Dictionary<string, EventSchema>();
        public Dictionary<string, CommandSchema> Commands { get; set; } = new Dictionary<string, CommandSchema>();
    }
}