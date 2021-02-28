using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo.AspNetCore.Mvc.Sample.Models.Hypermedia
{
    public class PersonRepresentation : Hal<Person>, 
                                        IHalLinks<Person>, 
                                        IHalEmbeddedAsync<Person>
    {
        public IEnumerable<Link> LinksFor(Person resource)
        {
            yield return new Link(Link.Self, $"/people/{resource.Id}");
        }

        public Task<object> EmbeddedForAsync(Person resource)
        {
            return Task.FromResult<object>(new
            {
                Hello = "World"
            });
        }
    }
    
    public class PersonListRepresentation : PagedListRepresentation<Person>
    {
        public PersonListRepresentation(PersonRepresentation personRepresentation) 
            : base("/people", personRepresentation) { }
    }
}