using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Cashier.Controllers
{
    /// <summary>
    /// Read only controller to get the history of known orders.
    /// </summary>
    [Route("order-history", Name = "GetOrderController")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class GetOrderController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        /// <summary>
        /// Constructor for GetOrderController.
        /// </summary>
        /// <param name="context">Database Context.</param>
        public GetOrderController(CoffeeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all known orders.
        /// </summary>
        /// <returns>List of orders.</returns>
        // GET: Orders
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            return await _context.Orders.Include(b => b.Coffees).ToListAsync();
        }

        /// <summary>
        /// Get the specified order.
        /// </summary>
        /// <param name="id">The orderId.</param>
        /// <returns>The specifed order, or a message stating the order was not found.</returns>
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
                return NotFound(ApiMessage.OrderNotFound(id));
            }

            return Ok(order);
        }
    }
}
