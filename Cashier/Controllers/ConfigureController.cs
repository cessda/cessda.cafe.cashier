using Cashier.Contexts;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cashier.Controllers
{
    /// <summary>
    /// Endpoint to allow configuration of known coffee machines
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ConfigureController : ControllerBase
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for the ConfigureController
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger</param>
        public ConfigureController(CoffeeDbContext context, ILogger<ConfigureController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Configure
        /// <summary>
        /// Get all the configured coffee machines.
        /// </summary>
        /// <returns>List of all configured coffee machines.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> GetMachines()
        {
            return await _context.Machines.ToListAsync().ConfigureAwait(true);
        }

        // POST: api/Configure
        /// <summary>
        /// Add a coffee machine to the list of known coffee machines.
        /// </summary>
        /// <param name="machines">A coffee machine.</param>
        /// <returns>The added coffee machine.</returns>
        [HttpPost]
        public async Task<ActionResult<Machine>> PostMachines(Machine machines)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (machines != null)
            {
                // If the coffee machine is not already configured
                if (_context.Machines.Find(machines.CoffeeMachine) == null)
                {
                    _context.Machines.Add(machines);
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.LogInformation("Added coffee machine " + machines.CoffeeMachine + ".");

                    return CreatedAtAction("GetMachines", new { id = machines.CoffeeMachine }, machines);
                }
            }

            return BadRequest();
        }

        // DELETE: api/Configure/5
        /// <summary>
        /// Remove a coffee machine from the list of known coffee machines
        /// </summary>
        /// <param name="url">The URL of the coffee machine to remove.</param>
        /// <returns>The removed coffee machine.</returns>
        [HttpDelete("{url}")]
        public async Task<ActionResult<Machine>> DeleteMachines(Uri url)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (url == null)
            {
                return BadRequest();
            }

            // Unescape the string
            url = new Uri(Uri.UnescapeDataString(url.ToString()));

            // Attempt to find the coffee machine
            var machines = await _context.Machines.FindAsync(url.ToString()).ConfigureAwait(true);
            if (machines == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machines);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _logger.LogInformation("Removed coffee machine " + url + ".");

            return machines;
        }
    }
}
