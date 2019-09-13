using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cashier.Contexts;
using Cashier.Models;

namespace Cashier.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ConfigureController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        public ConfigureController(CoffeeDbContext context)
        {
            _context = context;
        }

        // GET: api/Configure
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machines>>> GetMachines()
        {
            return await _context.Machines.ToListAsync();
        }

        // POST: api/Configure
        [HttpPost]
        public async Task<ActionResult<Machines>> PostMachines(Machines machines)
        {
            _context.Machines.Add(machines);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachines", new { id = machines.CoffeeMachine }, machines);
        }

        // DELETE: api/Configure/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Machines>> DeleteMachines(string id)
        {
            var machines = await _context.Machines.FindAsync(id);
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
