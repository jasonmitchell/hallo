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

        object IHal.StateFor(object resource)
        {
            var typedResource = (T) resource;
            return StateFor(typedResource);
        }

        object IHal.EmbeddedFor(object resource)
        {
            var typedResource = (T) resource;
            return EmbeddedFor(typedResource);
        }

        IEnumerable<Link> IHal.LinksFor(object resource)
        {
            var typedResource = (T) resource;
            return LinksFor(typedResource);
        }

        protected virtual object StateFor(T resource) => resource;
        protected virtual object EmbeddedFor(T resource) => null;
        protected virtual IEnumerable<Link> LinksFor(T resource) => null;
    }
}