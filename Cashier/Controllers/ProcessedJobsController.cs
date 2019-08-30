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
    [Route("processed-jobs")]
    [ApiController]
    public class ProcessedJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        public ProcessedJobsController(CoffeeDbContext context)
        {
            _context = context;
        }

        // GET: processed-jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coffee>>> GetCoffees()
        {
            var coffees = await _context.Coffees.ToListAsync();

            var processedCoffees = new List<Coffee>();

            // For each coffee check if they are processed
            foreach (var coffee in coffees)
            {
                if (coffee.State == ECoffeeState.PROCESSED)
                {
                    processedCoffees.Add(coffee);
                }
            }

            return processedCoffees;
        }

        // GET: processed-jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coffee>> GetCoffee(Guid id)
        {
            var coffee = await _context.Coffees.FindAsync(id);

            if (coffee != null)
            {
                // Only return coffees that are processed
                if (coffee.State == ECoffeeState.PROCESSED)
                {
                    return coffee;
                }
            }

            return NotFound();
        }

        private bool CoffeeExists(Guid id)
        {
            return _context.Coffees.Any(e => e.JobId == id);
        }
    }
}
