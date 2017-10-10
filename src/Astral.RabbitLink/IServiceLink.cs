using Astral.Logging;
using Astral.RabbitLink.Descriptions;
using RabbitLink;

namespace Astral.RabbitLink
{
    /// <summary>
    /// Service Link
    /// </summary>
    public interface IServiceLink : ILink
    {
        /// <summary>
        /// description factory
        /// </summary>
        IDescriptionFactory DescriptionFactory { get; }
        /// <summary>
        /// payload manager
        /// </summary>
        IPayloadManager PayloadManager { get; }
        /// <summary>
        /// service builder provider
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>service builder</returns>
        IServiceBuilder<TService> Service<TService>();
        
        /// <summary>
        /// service application holder name
        /// </summary>
        string HolderName { get; }
        
        ILogFactory LogFactory { get; }
    }
}