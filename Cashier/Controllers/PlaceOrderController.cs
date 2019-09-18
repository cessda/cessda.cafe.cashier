using Cashier.Contexts;
using Cashier.Engine;
using Cashier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cashier.Controllers
{
    [Route("place-order")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PlaceOrderController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        public PlaceOrderController(CoffeeDbContext context)
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
                return NotFound(OrderNotFound(id));
            }

            return Ok(order);
        }

        /// <summary>
        /// Update the specified order.
        /// </summary>
        /// <param name="id">The orderId to update.</param>
        /// <param name="order">The modified order.</param>
        /// <returns>The modified order, or a message if no coffees are specified.</returns>
        // PUT: Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] Guid id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderId)
            {
                return BadRequest();
            }

            // Add up the total amount of coffees ordered
            foreach (Coffee coffee in order.Coffees)
            {
                order.OrderSize += coffee.OrderSize;
            }

            // If no coffees have been sent reject the order
            if (order.OrderSize == 0)
            {
                return BadRequest(NoCoffees());
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound(OrderNotFound(id));
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The created order, or a message if an error occurs.</returns>
        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add up the total amount of coffees ordered
            foreach (Coffee coffee in order.Coffees)
            {
                // Each coffee should have an amount greater than 0
                if (coffee.OrderSize <= 0)
                {
                    return BadRequest(NoCoffees());
                }
                order.OrderSize += coffee.OrderSize;
            }

            // If no coffees have been sent reject the order
            if (order.OrderSize <= 0)
            {
                return BadRequest(NoCoffees());
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return the created order
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        /// <summary>
        /// Deletes the specified order.
        /// </summary>
        /// <param name="id">The orderId to delete.</param>
        /// <returns>The deleted order.</returns>
        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(OrderNotFound(id));
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        private static ApiMessage NoCoffees()
        {
            return new ApiMessage() { Message = "There must be at least one coffee in the order" };
        }

        private static ApiMessage OrderNotFound(Guid id)
        {
            return new ApiMessage() { Message = "The order " + id.ToString() + " was not found." };
        }
    }
}