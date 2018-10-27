using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo
{
    public abstract class Hal<T>  : IHal
    {
        async Task<HalRepresentation> IHal.RepresentationOfAsync(object resource)
        {
            var typedResource = (T) resource;

            var state = await StateFor(this, typedResource);
            var embedded = await EmbeddedFor(this, typedResource);
            var links = await LinksFor(this, typedResource);
            
            return new HalRepresentation(state, embedded, links);
        }

        private static async Task<object> StateFor(IHal representation, T resource)
        {
            if (representation is IHalStateAsync<T> asyncState)
            {
                return await asyncState.StateForAsync(resource);
            }

            if (representation is IHalState<T> state)
            {
                return state.StateFor(resource);
            }

            return resource;
        }

        private static async Task<object> EmbeddedFor(IHal representation, T resource)
        {
            if (representation is IHalEmbeddedAsync<T> asyncEmbedded)
            {
                return await asyncEmbedded.EmbeddedForAsync(resource);
            }

            if (representation is IHalEmbedded<T> embedded)
            {
                return embedded.EmbeddedFor(resource);
            }

            return null;
        }

        private static async Task<IEnumerable<Link>> LinksFor(IHal representation, T resource)
        {
            if (representation is IHalLinksAsync<T> asyncLinks)
            {
                return await asyncLinks.LinksForAsync(resource);
            }

            if (representation is IHalLinks<T> links)
            {
                return links.LinksFor(resource);
            }

            return null;
        }
    }
}