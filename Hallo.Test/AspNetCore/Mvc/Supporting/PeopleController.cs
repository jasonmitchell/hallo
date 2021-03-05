using Hallo.Test.AspNetCore.Supporting.Api;
using Microsoft.AspNetCore.Mvc;

namespace Hallo.Test.AspNetCore.Mvc.Supporting
{
    [Route("people")]
    public class PeopleController : ControllerBase
    {
        [Route("{id}")]
        public ActionResult<Person> Get(int id) => new Person
        {
            Id = id,
            FirstName = "Test",
            LastName = "User"
        };

        [Route("{id}/address")]
        public ActionResult<Address> GetAddress() => new Address
        {
            FirstLine = "Some First Line",
            SecondLine = "Some Second Line"
        };
    }
}