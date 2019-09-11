using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashier.Contexts;
using Cashier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cashier.Controllers
{
    [Route("order-history")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class GetOrderController : Controller
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        public GetOrderController(CoffeeDbContext context, ILogger<GetOrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Orders
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders.Include(b => b.Coffees);
        }

        // GET: Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.Include(b => b.Coffees).SingleAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }  
    }
}
