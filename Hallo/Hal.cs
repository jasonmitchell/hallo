using System.Collections.Generic;

namespace Hallo
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
        protected virtual IEnumerable<Link> LinksFor(T resource) => null;
    }
}