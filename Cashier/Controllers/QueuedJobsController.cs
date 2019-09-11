using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cashier.Contexts;
using Cashier.Models;
using Microsoft.Extensions.Logging;

namespace Cashier.Controllers
{
    [Route("queued-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class QueuedJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        public QueuedJobsController(CoffeeDbContext context, ILogger<QueuedJobsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: queued-jobs
        /// <summary>
        /// Get a list of queued jobs
        /// </summary>
        /// <returns>List of queued jobs</returns>
        [HttpGet]
        public async Task<ActionResult<CoffeeCount>> GetCoffees()
        {
            var coffees = await _context.Coffees.ToListAsync();

            var queuedCoffees = new List<Coffee>();

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

        // GET: queued-jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coffee>> GetCoffee(Guid id)
        {
            var coffee = await _context.Coffees.FindAsync(id);

            if (coffee != null)
            {
                // Only return coffees that are queued
                if (coffee.State == ECoffeeState.QUEUED)
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
