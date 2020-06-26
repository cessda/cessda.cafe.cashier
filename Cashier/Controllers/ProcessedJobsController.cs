using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier.Controllers
{
    /// <summary>
    /// Get processed jobs
    /// </summary>
    [Route("processed-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProcessedJobsController : ControllerBase
    {
        private readonly CashierDbContext _context;

        /// <summary>
        /// Constructor for ProcessedJobsController.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ProcessedJobsController(CashierDbContext context)
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
            // For each coffee check if they are processed
            return await _context.Jobs
                .Where(c => c.Machine != null)
                .ToListAsync();
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
            var coffee = await _context.Jobs.FindAsync(id);

            // Only return coffees that are processed
            if (coffee.Machine != null)
            {
                return coffee;
            }

            return NotFound();
        }
    }
}
