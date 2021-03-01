using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo.Test.AspNetCore.Mvc.Supporting
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
                yield return new Link("self", $"/people/{resource.Id}", "application/hal+json",
                    new Uri("http://example.com/deprecated"), "a name", new Uri("http://example.com/profile"),
                    "A Title", "en-IE");
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