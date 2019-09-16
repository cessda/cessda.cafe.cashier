using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cashier.Contexts;
using Cashier.Models;
using Microsoft.Extensions.Logging;

namespace Cashier.Controllers
{
    [Route("place-order")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PlaceOrderController : ControllerBase
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        public PlaceOrderController(CoffeeDbContext context, ILogger<PlaceOrderController> logger)
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
                return BadRequest("There must be at least one coffee in the order");
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

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
                    return BadRequest("There must be at least one coffee in the order");
                }
                order.OrderSize += coffee.OrderSize;
            }

            // If no coffees have been sent reject the order
            if (order.OrderSize <= 0)
            {
                return BadRequest("There must be at least one coffee in the order");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Start processing the order
            new Engine.OrderEngine(_context, _logger).StartOrder(order.OrderId);

            // Return the created order
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

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
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}