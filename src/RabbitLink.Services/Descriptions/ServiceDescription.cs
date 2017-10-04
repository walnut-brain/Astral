using System.Collections.Generic;

namespace RabbitLink.Services.Descriptions
{
    public class ServiceDescription
    {
        public ServiceDescription(string name, string owner)
        {
            Name = name;
            Owner = owner;
        }

        public string Name { get; }
        public string Owner { get; }
        public Dictionary<string, EventDescription> Events { get; set; } = new Dictionary<string, EventDescription>();
    }
}