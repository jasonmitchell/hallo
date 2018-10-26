using System.Collections.Generic;

namespace Hallo
{
    public interface IHal
    {
        HalRepresentation RepresentationOf(object resource);

        object StateFor(object resource);
        object EmbeddedFor(object resource);
        IEnumerable<Link> LinksFor(object resource);
    }
}