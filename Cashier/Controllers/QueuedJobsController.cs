using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cashier.Controllers
{
    /// <summary>
    /// Controller to fetch all queued jobs.
    /// </summary>
    [Route("queued-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class QueuedJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;

        /// <summary>
        /// Constructor for QueuedJobsController.
        /// </summary>
        /// <param name="context">Database context.</param>
        public QueuedJobsController(CoffeeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of queued jobs
        /// </summary>
        /// <returns>List of queued jobs</returns>
        // GET: queued-jobs
        [HttpGet]
        public async Task<ActionResult<CoffeeCount>> GetCoffees()
        {
            // For each coffee check if they are processed
            var coffees = await _context.Coffees
                .Where(c => c.State == ECoffeeState.QUEUED)
                .ToListAsync();

            return new CoffeeCount(coffees);
        }

        /// <summary>
        /// Get an individual queued job.
        /// </summary>
        /// <param name="id">The jobId to get.</param>
        /// <returns>The queued job.</returns>
        // GET: queued-jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetCoffee(Guid id)
        {
            var coffee = await _context.Coffees.FindAsync(id);

            if (coffee?.State == ECoffeeState.QUEUED)
            {
                // Only return coffees that are queued
                return coffee;
            }
            return NotFound();
        }
    }
}
