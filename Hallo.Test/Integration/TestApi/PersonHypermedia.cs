using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallo.Test.Integration.TestApi
{
    public class PersonRepresentation : Hal<Person>,
                                        IHalLinks<Person>,
                                        IHalEmbeddedAsync<Person>
    {
        private readonly ContactLookup _contacts;

        public PersonRepresentation(ContactLookup contacts)
        {
            _contacts = contacts;
        }
    
        public IEnumerable<Link> LinksFor(Person resource)
        {
            if (resource.Id != 404)
            {
                yield return new Link("self", $"/people/{resource.Id}");
            }
        }

        public async Task<object> EmbeddedForAsync(Person resource)
        {
            return new
            {
                contacts = await _contacts.For(resource)
            };
        }
    }
}