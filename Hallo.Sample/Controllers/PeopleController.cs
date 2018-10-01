using System.Collections.Generic;
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
        public ActionResult<IEnumerable<Person>> List()
        {
            return _repository.List();
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Person> Get(int id)
        {
            return _repository.Get(id);
        }
    }
}