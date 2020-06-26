using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier.Controllers
{
    /// <summary>
    /// Endpoint to allow configuration of known coffee machines
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ConfigureController : ControllerBase
    {
        private readonly CashierDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for the ConfigureController
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger</param>
        public ConfigureController(CashierDbContext context, ILogger<ConfigureController> logger)
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
            return await _context.Machines.ToListAsync();
        }

        // POST: api/Configure
        /// <summary>
        /// Add a coffee machine to the list of known coffee machines.
        /// </summary>
        /// <param name="machine">A coffee machine.</param>
        /// <returns>The added coffee machine.</returns>
        [HttpPost]
        public async Task<ActionResult<Machine>> PostMachines(Machine machine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (machine != null && Uri.IsWellFormedUriString(machine.CoffeeMachine, UriKind.Absolute)
                && _context.Machines.Find(machine.CoffeeMachine) == null)
            {
                // If the coffee machine is not already configured
                _context.Machines.Add(machine);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added coffee machine " + machine.CoffeeMachine + ".");

                return CreatedAtAction("GetMachines", new { id = machine.CoffeeMachine }, machine);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings",
            Justification = "Validation is performed before any operation is performed")]
        public async Task<ActionResult<Machine>> DeleteMachines(string url)
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
            url = Uri.UnescapeDataString(url);

            // Attempt to find the coffee machine
            var machines = await _context.Machines.FindAsync(url);
            if (machines == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machines);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Removed coffee machine " + url + ".");

            return machines;
        }
    }
}
