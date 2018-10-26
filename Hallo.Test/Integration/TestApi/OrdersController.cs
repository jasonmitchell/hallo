using System;
using Microsoft.AspNetCore.Mvc;

namespace Hallo.Test.Integration.TestApi
{
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        [Route("{id}")]
        public ActionResult<Order> Get(int id) => new Order
        {
            Id = id,
            Placed = DateTime.UtcNow
        };
    }
}