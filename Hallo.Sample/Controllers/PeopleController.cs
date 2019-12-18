using Hallo.Sample.Data;
using Hallo.Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hallo.Sample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PeopleRepository _repository;

        public PeopleController(PeopleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<PagedList<Person>> List([FromQuery]Paging paging)
        {
            return _repository.List(paging);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Person> Get(int id)
        {
            return _repository.Get(id);
        }
    }
}