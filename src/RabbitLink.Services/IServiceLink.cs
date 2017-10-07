using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
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
    }
}