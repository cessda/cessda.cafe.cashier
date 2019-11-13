using Cashier.Contexts;
using Cashier.Engine;
using Cashier.Exceptions;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cashier.Controllers
{
    /// <summary>
    /// Controller to tell the Cashier to process orders stored.
    /// </summary>
    [Route("process-jobs")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProcessJobsController : ControllerBase
    {
        private readonly CoffeeDbContext _context;
        private readonly IOrderEngine _orderEngine;

        /// <summary>
        /// Constructor for ProcessJobsController.
        /// </summary>
        /// <param name="context">Database Context.</param>
        /// <param name="orderEngine">Engine to process orders.</param>
        public ProcessJobsController(CoffeeDbContext context, IOrderEngine orderEngine)
        {
            _context = context;
            _orderEngine = orderEngine;
        }

        // POST: api/ProcessJobs
        /// <summary>
        /// Process all coffees
        /// </summary>
        /// <returns>Message</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessage), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiMessage>> PostCoffee()
        {
            var coffees = await _context.Jobs.ToListAsync().ConfigureAwait(true);

            foreach (var coffee in coffees)
            {
                try
                {
                    await _orderEngine.StartJobAsync(coffee.JobId).ConfigureAwait(false);
                }
                catch (NoCoffeeMachinesException e)
                {
                    return StatusCode(500, new ApiMessage { Message = e.Message });
                }
            }

            // Count the coffees in either state
            var jobsDeployed = await _context.Jobs.CountAsync(c => c.State == ECoffeeState.PROCESSED).ConfigureAwait(true);
            var jobsQueued = await _context.Jobs.CountAsync(c => c.State == ECoffeeState.QUEUED).ConfigureAwait(true);

            return Ok(new ApiMessage { Message = jobsDeployed + " jobs deployed, " + jobsQueued + " jobs still queued." });
        }
    }
}
