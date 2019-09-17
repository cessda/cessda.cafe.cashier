using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cashier.Contexts;
using Cashier.Models;
using System.Web;

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

        /// <summary>
        /// Constructor for the ConfigureController
        /// </summary>
        /// <param name="context">Database context</param>
        public ConfigureController(CoffeeDbContext context)
        {
            _context = context;
        }

        // GET: api/Configure
        /// <summary>
        /// Get all the configured coffee machines.
        /// </summary>
        /// <returns>List of all configured coffee machines.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machines>>> GetMachines()
        {
            return await _context.Machines.ToListAsync();
        }

        // POST: api/Configure
        /// <summary>
        /// Add a coffee machine to the list of known coffee machines.
        /// </summary>
        /// <param name="machines">A coffee machine.</param>
        /// <returns>The added coffee machine.</returns>
        [HttpPost]
        public async Task<ActionResult<Machines>> PostMachines(Machines machines)
        {
            _context.Machines.Add(machines);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachines", new { id = machines.CoffeeMachine }, machines);
        }

        // DELETE: api/Configure/5
        /// <summary>
        /// Remove a coffee machine from the list of known coffee machines
        /// </summary>
        /// <param name="id">The URL of the coffee machine to remove.</param>
        /// <returns>The removed coffee machine.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Machines>> DeleteMachines(string id)
        {
            var machines = await _context.Machines.FindAsync(HttpUtility.UrlDecode(id));
            if (machines == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machines);
            await _context.SaveChangesAsync();

            return machines;
        }
    }
}
