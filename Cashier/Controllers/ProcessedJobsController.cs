using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cashier.Contexts;
using Microsoft.Extensions.Logging;
using Cashier.Models.Database;

namespace Cashier.Controllers
{
    /// <summary>
    /// Get processed jobs
    /// </summary>
    [Route("processed-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProcessedJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        /// <summary>
        /// Constructor for ProcessedJobsController.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ProcessedJobsController(CoffeeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all processed jobs.
        /// </summary>
        /// <returns>List of processed jobs.</returns>
        // GET: processed-jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetCoffees()
        {
            var coffees = await _context.Coffees.ToListAsync();

            var processedCoffees = new List<Job>();

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

        /// <summary>
        /// Get a single processed job.
        /// </summary>
        /// <param name="id">The coffee to find.</param>
        /// <returns>A coffee</returns>
        // GET: processed-jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetCoffee(Guid id)
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
    }
}
