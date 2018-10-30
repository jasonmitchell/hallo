using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo
{
    /// <summary>
    /// Provides generation of links for HAL documents
    /// </summary>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalLinks<in TResource>
    {
        /// <summary>
        /// Generates links for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to create links for</param>
        /// <returns>The resource links</returns>
        IEnumerable<Link> LinksFor(TResource resource);
    }

    /// <summary>
    /// Provides asynchronous generation of links for HAL documents
    /// </summary>
    /// <remarks>
    /// Asynchronous operations take precedence over synchronous operations
    /// when both interfaces are implemented.
    /// </remarks>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalLinksAsync<in TResource>
    {
        /// <summary>
        /// Generates links for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to create links for</param>
        /// <returns>The (awaitable) resource links</returns>
        Task<IEnumerable<Link>> LinksForAsync(TResource resource);
    }
}