using Cashier.Contexts;
using Cashier.Engine;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly CashierDbContext _context;
        private readonly ILogger _logger;
        private readonly ICoffeeMachineService _coffeeMachineService;

        /// <summary>
        /// Constructor for PlaceOrderController
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="coffeeMachineService">Order Engine.</param>
        /// <param name="logger">Logger.</param>
        public PlaceOrderController(CashierDbContext context, ILogger<PlaceOrderController> logger, ICoffeeMachineService coffeeMachineService)
        {
            _context = context;
            _logger = logger;
            _coffeeMachineService = coffeeMachineService;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">The order.</param>
        /// <returns>The created order, or a message if an error occurs.</returns>
        // POST: api/Orders
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
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
                    order.Jobs.Add(new Job(order.OrderPlaced)
                    {
                        Product = coffee.Product,
                        OrderSize = order.OrderSize
                    });
                    _logger.LogDebug("Added coffee {product} to the order.", coffee.Product);
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created order {orderId}.", order.OrderId);

            await _coffeeMachineService.StartAllJobsAsync();

            // Return the created order
            return CreatedAtRoute(nameof(GetOrderController), new { id = order.OrderId }, order);
        }

        /// <summary>
        /// Deletes the specified order.
        /// </summary>
        /// <param name="id">The orderId to delete.</param>
        /// <returns>The deleted order.</returns>
        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(ApiMessage.OrderNotFound(id));
            }

            // Check if any jobs have been sent to a coffee machine
            foreach (var job in order.Jobs)
            {
                if (!string.IsNullOrEmpty(job.Machine))
                {
                    return BadRequest(ApiMessage.OrderAlreadyProcessed(order.OrderId));
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted order {orderId}.", order.OrderId);

            return Ok(order);
        }
    }
}