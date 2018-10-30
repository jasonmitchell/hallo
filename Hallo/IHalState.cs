using System.Threading.Tasks;

namespace Hallo
{
    /// <summary>
    /// Provides generation of state for HAL documents
    /// </summary>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalState<in TResource>
    {
        /// <summary>
        /// Generates state for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to derive state from</param>
        /// <returns>The derived state</returns>
        object StateFor(TResource resource);
    }
    
    /// <summary>
    /// Provides asynchronous generation of state for HAL documents
    /// </summary>
    /// <remarks>
    /// Asynchronous operations take precedence over synchronous operations
    /// when both interfaces are implemented.
    /// </remarks>
    /// <typeparam name="TResource">The type of resource the generator applies to</typeparam>
    public interface IHalStateAsync<in TResource>
    {
        /// <summary>
        /// Generates state for the HAL document
        /// </summary>
        /// <param name="resource">The instance of the resource to derive state from</param>
        /// <returns>The (awaitable) derived state</returns>
        Task<object> StateForAsync(TResource resource);
    }
}