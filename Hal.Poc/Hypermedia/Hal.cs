using System.Collections.Generic;

namespace Hal.Poc.Hypermedia
{
    public abstract class Hal<T> : IHal
    {
        HalRepresentation IHal.RepresentationOf(object resource)
        {
            var typedResource = (T) resource;
            var state = StateFor(typedResource);
            var embedded = EmbeddedFor(typedResource);
            var links = LinksFor(typedResource);
            
            return new HalRepresentation(state, embedded, links);
        }

        protected virtual object StateFor(T resource) => resource;
        protected virtual object EmbeddedFor(T resource) => null;
        protected abstract IEnumerable<Link> LinksFor(T resource);
    }
}