using System;
using System.Collections.Concurrent;
using System.Net.Mime;
using System.Reflection;
using Astral.Liaison;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using Astral.Schema;
using RabbitLink.Topology;

namespace Astral.RabbitLink.Descriptions
{
    /// <summary>
    /// Default description factory realization for astral markup
    /// </summary>
    public class DefaultDescriptionFactory : IDescriptionFactory
    {
        private readonly bool _useMemberNames;

        public DefaultDescriptionFactory(bool useMemberNames)
        {
            _useMemberNames = useMemberNames;
        }

        /// <summary>
        /// Generate service description for type
        /// </summary>
        /// <param name="type">service interface type</param>
        /// <returns>service description</returns>
        protected virtual IServiceSchema GenerateDescription(Type type)
            => ServiceSchema.FromType(type, _useMemberNames);
        
        
        private readonly ConcurrentDictionary<Type, IServiceSchema> _cache = new ConcurrentDictionary<Type, IServiceSchema>();

        public IServiceSchema GetDescription(Type type)
        {
            return _cache.GetOrAdd(type, GenerateDescription);
        }
    }
}