using System.Collections.Generic;

namespace Hallo.Test.Integration.TestApi
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    
    public class PersonRepresentation : Hal<Person>
    {
        private readonly ContactLookup _contacts;

        public PersonRepresentation(ContactLookup contacts)
        {
            _contacts = contacts;
        }
        
        protected override IEnumerable<Link> LinksFor(Person resource)
        {
            yield return new Link("self", $"/people/{resource.Id}");
        }

        protected override object EmbeddedFor(Person resource)
        {
            return new
            {
                contacts = _contacts.For(resource)
            };
        }
    }
}