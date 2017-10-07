using System.Collections.Generic;

namespace RabbitLink.Services.Descriptions
{
    /// <summary>
    /// service description
    /// </summary>
    public class ServiceDescription
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">service name</param>
        /// <param name="owner">service owner</param>
        public ServiceDescription(string name, string owner)
        {
            Name = name;
            Owner = owner;
        }

        /// <summary>
        /// service name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// service owner
        /// </summary>
        public string Owner { get; }
        /// <summary>
        /// events dictionary
        /// </summary>
        public Dictionary<string, EventDescription> Events { get; set; } = new Dictionary<string, EventDescription>();
        /// <summary>
        /// calls dictionary
        /// </summary>
        public Dictionary<string, CallDescription> Calls { get; set; } = new Dictionary<string, CallDescription>();
    }
}