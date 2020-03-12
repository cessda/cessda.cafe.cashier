using Cashier.Contexts;
using Cashier.Engine;
using Cashier.Exceptions;
using Cashier.Models;
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
        private readonly CashierDbContext _context;
        private readonly ICoffeeMachineService _coffeeMachineService;

        /// <summary>
        /// Constructor for ProcessJobsController.
        /// </summary>
        public ProcessJobsController(CashierDbContext context, ICoffeeMachineService coffeeMachineService)
        {
            _context = context;
            _coffeeMachineService = coffeeMachineService;
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
            try
            {
                await _coffeeMachineService.StartAllJobsAsync();
            }
            catch (NoCoffeeMachinesException e)
            {
                return StatusCode(500, new ApiMessage { Message = e.Message });
            }

            // Count the coffees in either state
            int jobsDeployed = await _context.Jobs.CountAsync(c => !string.IsNullOrEmpty(c.Machine));
            int jobsQueued = await _context.Jobs.CountAsync(c => string.IsNullOrEmpty(c.Machine));

            return Ok(new ApiMessage { Message = jobsDeployed + " jobs deployed, " + jobsQueued + " jobs still queued." });
        }
    }
}
