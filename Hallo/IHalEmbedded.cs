using System.Threading.Tasks;

namespace Hallo
{
    /// <summary>
    /// Provides generation of embedded resources for HAL documents
    /// </summary>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalEmbedded<in TResource>
    {
        /// <summary>
        /// Generates embedded resources for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to generate embedded resources for</param>
        /// <returns>The embedded resources</returns>
        object EmbeddedFor(TResource resource);
    }

    /// <summary>
    /// Provides asynchronous generation of embedded resources for HAL documents
    /// </summary>
    /// <remarks>
    /// Asynchronous operations take precedence over synchronous operations
    /// when both interfaces are implemented.
    /// </remarks>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalEmbeddedAsync<in TResource>
    {
        /// <summary>
        /// Generates embedded resources for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to generate embedded resources for</param>
        /// <returns>The (awaitable) embedded resources</returns>
        Task<object> EmbeddedForAsync(TResource resource);
    }
}