using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Models;
using Cessda.Cafe.Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier.Controllers
{
    /// <summary>
    /// Controller to fetch all queued jobs.
    /// </summary>
    [Route("queued-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class QueuedJobsController : ControllerBase
    {
        private readonly CashierDbContext _context;

        /// <summary>
        /// Constructor for QueuedJobsController.
        /// </summary>
        /// <param name="context">Database context.</param>
        public QueuedJobsController(CashierDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of queued jobs
        /// </summary>
        /// <returns>List of queued jobs</returns>
        // GET: queued-jobs
        [HttpGet]
        public ActionResult<CoffeeCount> GetCoffees()
        {
            // For each coffee check if they are processed
            var coffees = _context.Jobs.Where(c => string.IsNullOrEmpty(c.Machine));

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
            var coffee = await _context.Jobs.FindAsync(id);

            // Only return coffees that are queued
            if (string.IsNullOrEmpty(coffee.Machine))
            {
                return coffee;
            }
            return NotFound();
        }
    }
}
