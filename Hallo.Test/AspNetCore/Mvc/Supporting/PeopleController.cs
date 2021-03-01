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
    }
}