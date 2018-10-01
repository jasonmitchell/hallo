using System.Collections.Generic;

namespace Hallo.Sample.Models.Hypermedia
{
    public class PersonRepresentation : Hal<Person>
    {
        protected override IEnumerable<Link> LinksFor(Person resource)
        {
            yield return new Link(Link.Self, $"/people/{resource.Id}");
        }
    }
}