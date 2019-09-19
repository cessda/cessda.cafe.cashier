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
    /// <summary>
    /// Write only controller to create new orders, or delete orders that have not been processed yet.
    /// </summary>
    [Route("place-order")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PlaceOrderController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        /// <summary>
        /// Constructor for PlaceOrderController
        /// </summary>
        /// <param name="context">Database context.</param>
        public PlaceOrderController(CoffeeDbContext context)
        {
            _context = context;
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
                    return BadRequest(ApiMessage.NoCoffees());
                }
                order.OrderSize += coffee.OrderSize;
            }

            // If no coffees have been sent reject the order
            if (order.OrderSize <= 0)
            {
                return BadRequest(ApiMessage.NoCoffees());
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return the created order
            return CreatedAtRoute("GetOrderController", new { id = order.OrderId }, order);
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
                return NotFound(ApiMessage.OrderNotFound(id));
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }
    }
}