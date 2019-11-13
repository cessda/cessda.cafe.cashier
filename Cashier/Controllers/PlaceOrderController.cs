using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System;
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
        /// <param name="request">The order.</param>
        /// <returns>The created order, or a message if an error occurs.</returns>
        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] CoffeeRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var order = new Order();

            // Add up the total amount of coffees ordered
            foreach (var coffee in request.Coffees)
            {
                // Each coffee should have an amount greater than 0
                if (coffee.Count <= 0)
                {
                    return BadRequest(ApiMessage.NoCoffees());
                }

                order.OrderSize += coffee.Count;
            }

            // If no coffees have been sent reject the order
            if (order.OrderSize <= 0)
            {
                return BadRequest(ApiMessage.NoCoffees());
            }

            // Add each coffee to the order
            foreach (var coffee in request.Coffees)
            {
                for (int i = 0; i < coffee.Count; i++)
                {
                    order.Jobs.Add(new Job()
                    {
                        Product = coffee.Product,
                        OrderSize = order.OrderSize
                    });
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync().ConfigureAwait(false);

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
            var order = await _context.Orders.FindAsync(id).ConfigureAwait(true);
            if (order == null)
            {
                return NotFound(ApiMessage.OrderNotFound(id));
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(order);
        }
    }
}