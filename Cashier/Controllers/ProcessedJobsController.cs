﻿using Cashier.Contexts;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            // For each coffee check if they are processed
            return await _context.Coffees
                .Where(c => c.State == ECoffeeState.PROCESSED)
                .ToListAsync().ConfigureAwait(true);
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
            var coffee = await _context.Coffees.FindAsync(id).ConfigureAwait(true);

            // Only return coffees that are processed
            if (coffee?.State == ECoffeeState.PROCESSED)
            {
                return coffee;
            }

            return NotFound();
        }
    }
}
