using System;
using Astral.Schema;

namespace Astral.RabbitLink.Descriptions
{
    /// <summary>
    /// Extract service description from type
    /// </summary>
    public interface IDescriptionFactory
    {
        /// <summary>
        /// extract service description from type
        /// </summary>
        /// <param name="type">service type</param>
        /// <returns>service description</returns>
        IComplexServiceSchema GetDescription(Type type);
    }
}