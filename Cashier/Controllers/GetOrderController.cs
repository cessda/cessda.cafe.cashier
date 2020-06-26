using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Models;
using Cessda.Cafe.Cashier.Models.Database;
using Cessda.Cafe.Cashier.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier.Controllers
{
    /// <summary>
    /// Read only controller to get the history of known orders.
    /// </summary>
    [Route("order-history", Name = nameof(GetOrderController))]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class GetOrderController : ControllerBase
    {
        private readonly CashierDbContext _context;
        private readonly ICoffeeMachineService _orderEngine;

        /// <summary>
        /// Constructor for GetOrderController.
        /// </summary>
        public GetOrderController(CashierDbContext context, ICoffeeMachineService orderEngine)
        {
            _context = context;
            _orderEngine = orderEngine;
        }

        /// <summary>
        /// Get a list of all known orders.
        /// </summary>
        /// <returns>List of orders.</returns>
        // GET: Orders
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            return await _context.Orders.Include(b => b.Jobs).ToListAsync();
        }

        /// <summary>
        /// Get the specified order.
        /// </summary>
        /// <param name="id">The orderId.</param>
        /// <returns>The specified order, or a message stating the order was not found.</returns>
        // GET: Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var order = await _context.Orders.Include(b => b.Jobs).SingleAsync(o => o.OrderId == id);
                await _orderEngine.StartOrderAsync(id);
                return Ok(order);
            }
            catch (InvalidOperationException)
            {
                return NotFound(ApiMessage.OrderNotFound(id));
            }
        }
    }
}
