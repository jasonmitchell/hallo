using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallo
{
    /// <summary>
    /// A base class for defining HAL representations of a resource type
    /// </summary>
    /// <typeparam name="TResource">The resource type this representation applies to</typeparam>
    public abstract class Hal<TResource>  : IHal
    {
        /// <inheritdoc cref="IHal"/>
        async Task<HalRepresentation> IHal.RepresentationOfAsync(object resource)
        {
            var typedResource = (TResource) resource;

            var state = await StateFor(this, typedResource);
            var embedded = await EmbeddedFor(this, typedResource);
            var links = await LinksFor(this, typedResource);
            
            return new HalRepresentation(state, embedded, links);
        }

        private static async Task<object> StateFor(IHal representation, TResource resource)
        {
            if (representation is IHalStateAsync<TResource> asyncState)
            {
                return await asyncState.StateForAsync(resource);
            }

            if (representation is IHalState<TResource> state)
            {
                return state.StateFor(resource);
            }

            return resource;
        }

        private static async Task<object> EmbeddedFor(IHal representation, TResource resource)
        {
            if (representation is IHalEmbeddedAsync<TResource> asyncEmbedded)
            {
                return await asyncEmbedded.EmbeddedForAsync(resource);
            }

            if (representation is IHalEmbedded<TResource> embedded)
            {
                return embedded.EmbeddedFor(resource);
            }

            return null;
        }

        private static async Task<IEnumerable<Link>> LinksFor(IHal representation, TResource resource)
        {
            if (representation is IHalLinksAsync<TResource> asyncLinks)
            {
                return await asyncLinks.LinksForAsync(resource);
            }

            if (representation is IHalLinks<TResource> links)
            {
                return links.LinksFor(resource);
            }

            return Enumerable.Empty<Link>();
        }
    }
}