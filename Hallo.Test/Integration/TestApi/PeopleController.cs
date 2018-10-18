using Microsoft.AspNetCore.Mvc;

namespace Hallo.Test.Integration.TestApi
{
    [Route("people")]
    public class PeopleController : ControllerBase
    {
        public ActionResult<PagedList<Person>> Get() => new PagedList<Person>
        {
            CurrentPage = 2,
            TotalItems = 3,
            TotalPages = 10,
            Items = new []
            {
                new Person
                {
                    Id = 123,
                    FirstName = "Test",
                    LastName = "User"
                },
                new Person
                {
                    Id = 456,
                    FirstName = "Another",
                    LastName = "TestUser"
                },
                new Person
                {
                    Id = 789,
                    FirstName = "Someone",
                    LastName = "Else"
                }
            }
        };
            
        [Route("{id}")]
        public ActionResult<Person> Get(int id) => new Person
        {
            Id = id,
            FirstName = "Test",
            LastName = "User"
        };
    }
}