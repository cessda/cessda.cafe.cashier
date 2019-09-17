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
using Cashier.Engine;
using Cashier.Exceptions;

namespace Cashier.Controllers
{
    [Route("process-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProcessJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;
        private readonly IOrderEngine _orderEngine;

        public ProcessJobsController(CoffeeDbContext context, IOrderEngine orderEngine)
        {
            _context = context;;
            _orderEngine = orderEngine;
        }

        // POST: api/ProcessJobs
        /// <summary>
        /// Process all coffees
        /// </summary>
        /// <returns>Message</returns>
        [HttpPost]
        public async Task<ActionResult<ApiMessage>> PostCoffee()
        {
            var coffees = await _context.Coffees.ToListAsync();

            foreach (var coffee in coffees)
            {
                try
                {
                    _orderEngine.StartCoffee(coffee.JobId);
                }
                catch (NoCoffeeMachinesException e)
                {
                    return StatusCode(500, new ApiMessage { Message = e.Message });
                }
            }

            // Update the local variable with changes from the database
            coffees = await _context.Coffees.ToListAsync();

            int jobsDeployed = 0;
            int jobsQueued = 0;

            foreach (var coffee in coffees)
            {
                if (coffee.State == ECoffeeState.PROCESSED)
                {
                    jobsDeployed++;
                }
                else if (coffee.State == ECoffeeState.QUEUED)
                {
                    jobsQueued++;
                }
            }
            return Ok(new ApiMessage { Message = jobsDeployed + " jobs deployed, "+ jobsQueued +" jobs still queued." });
        }
    }
}
