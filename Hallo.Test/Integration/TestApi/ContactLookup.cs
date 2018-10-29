using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo.Test.Integration.TestApi
{
    public class ContactLookup
    {
        public Task<IEnumerable<Person>> For(Person person)
        {
            return Task.FromResult<IEnumerable<Person>>(new[]
            {
                new Person
                {
                    Id = 99,
                    FirstName = "Contact",
                    LastName = "User"
                }
            });
        }
    }
}