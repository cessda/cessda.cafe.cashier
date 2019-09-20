using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            var coffees = await _context.Coffees.ToListAsync();

            var queuedCoffees = new List<Job>();

            // For each coffee check if they are processed
            foreach (var coffee in coffees)
            {
                if (coffee.State == ECoffeeState.QUEUED)
                {
                    queuedCoffees.Add(coffee);
                }
            }

            return new CoffeeCount(queuedCoffees);
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

            if ((coffee != null) && (coffee.State == ECoffeeState.QUEUED))
            {
                // Only return coffees that are queued
                return coffee;
            }
            return NotFound();
        }
    }
}
