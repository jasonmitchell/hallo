using System.Collections.Generic;

namespace Hallo.Test.Integration.TestApi
{
    public class ContactLookup
    {
        public IEnumerable<Person> For(Person person)
        {
            return new[]
            {
                new Person
                {
                    Id = 99,
                    FirstName = "Contact",
                    LastName = "User"
                }
            };
        }
    }
}