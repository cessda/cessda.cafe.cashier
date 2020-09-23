using CESSDA.Cafe.Cashier.Contexts;
using CESSDA.Cafe.Cashier.Exceptions;
using CESSDA.Cafe.Cashier.Models;
using CESSDA.Cafe.Cashier.Models.Database;
using CESSDA.Cafe.Cashier.Properties;
using CorrelationId.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CESSDA.Cafe.Cashier.Service
{
    /// <summary>
    /// Class to hold the logic of contacting coffee machines.
    /// </summary>
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly CashierDbContext _context;
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// Constructor for OrderEngine, used for passing the logger and the database context.
        /// </summary>
        public CoffeeMachineService(CashierDbContext context, HttpClient httpClient, ILogger<CoffeeMachineService> logger, ICorrelationContextAccessor correlationContext)
        {
            _correlationContext = correlationContext;
            _context = context;
            _httpClient = httpClient;
            _logger = logger;

            // Set up the JSON converter
            var jsonSettings = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            jsonSettings.Converters.Add(new StringEnumConverter());
            _jsonSettings = jsonSettings;
        }

        /// <summary>
        /// Starts the jobs associated with an order.
        /// </summary>
        /// <param name="id">The order to start.</param>
        public async Task StartOrderAsync(Guid id)
        {
            var order = await _context.Orders.Include(b => b.Jobs).SingleAsync(o => o.OrderId == id);
            await StartJobListAsync(order.Jobs);
        }

        /// <summary>
        /// Starts all jobs that haven't been started.
        /// </summary>
        public Task StartAllJobsAsync()
        {
            var jobs = _context.Jobs.Where(j => string.IsNullOrEmpty(j.Machine));
            return StartJobListAsync(jobs);
        }

        /// <summary>
        /// Starts the jobs specified in the job list.
        /// </summary>
        /// <param name="jobs">The list of jobs to start.</param>
        private Task StartJobListAsync(IEnumerable<Job> jobs)
        {
            var taskList = jobs.Select(job => StartJobAsync(job.JobId));
            return Task.WhenAll(taskList);
        }


        /// <summary>
        /// Start the specified coffee
        /// </summary>
        /// <param name="id">ID of coffee to start</param>
        public async Task StartJobAsync(Guid id)
        {
            // Load the coffee
            var job = await _context.Jobs.SingleAsync(c => c.JobId == id);

            // Check if the coffee has already been sent to a machine
            if (string.IsNullOrEmpty(job.Machine))
            {
                _logger.LogInformation("Starting job {jobId}.", job.JobId);

                // Set up web request
                bool success = false;
                var coffeeMachines = GetCoffeeMachinesAsync();

                var coffeePayload = new CoffeePayload()
                {
                    JobId = job.JobId,
                    Product = job.Product
                };

                string json = JsonConvert.SerializeObject(coffeePayload, _jsonSettings);
                _logger.LogDebug("Created JSON: {json}.", json);

                // Iterate through all known coffee machines
                foreach (var machine in await coffeeMachines)
                {
                    // Break if the order has already been sent to a machine
                    if (success)
                    {
                        break;
                    }

                    success = await SendRequestAsync(json, machine);

                    // Update local state to mark that the coffee was sent to a remote machine
                    if (success)
                    {
                        // Mark the time that the order was sent
                        _logger.LogInformation("Sent job {jobId} to machine {machine}.", job.JobId, machine);
                        job.SetJobStarted();
                        job.SetMachine(machine);

                        // Update the database
                        _context.Entry(job).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                // No coffee machines could accept the coffee
                if (!success)
                {
                    _logger.LogWarning("No coffee machines could accept job {jobId}.", job.JobId);
                }
            }
            else
            {
                _logger.LogWarning("Attempted to start coffee {jobId} which has already been started.", job.JobId);
            }
        }


        /// <summary>
        /// Sends a string to the specified endpoint.
        /// </summary>
        /// <param name="payload">The string to send.</param>
        /// <param name="uri">The URI to send to.</param>
        private async Task<bool> SendRequestAsync(string payload, Uri uri)
        {
            // Create a new WebRequest
            using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
            {
                // Forward the request id
                content.Headers.Add("X-Request-Id", _correlationContext.CorrelationContext.CorrelationId);

                // Submit the coffee to the coffee machine
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(new Uri(uri, "start-job"), content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response from the coffee machine
                        var responseString = response.Content.ReadAsStringAsync();
                        JsonConvert.DeserializeObject<Job>(await responseString);
                        if (_logger.IsEnabled(LogLevel.Trace))
                        {
                            _logger.LogTrace("Response from {CoffeeMachineUri}: {response}.", uri, responseString);
                        }

                        return true;
                    }
                    else
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        var responseCode = response.StatusCode;
                        try
                        {
                            var apiMessage = JsonConvert.DeserializeObject<ApiMessage>(stringResponse);
                            if (stringResponse == "Machine busy!")
                            {
                                _logger.LogInformation("Coffee machine {CoffeeMachineUri} is busy.", uri);
                            }
                            else
                            {
                                _logger.LogWarning("Coffee machine {CoffeeMachineUri} responded with code {code} and with message: {message}.", uri, (int)responseCode, apiMessage.Message);
                            }
                        }
                        catch (JsonReaderException)
                        {
                            _logger.LogWarning("Coffee machine {CoffeeMachineUri} responded with code {code}. The message could not be parsed.", uri, (int)responseCode);
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("Connecting to coffee machine {CoffeeMachineUri} failed: {e}.", uri, e.Message);
                }
            }
            return false;
        }


        /// <summary>
        /// Gets a list of known coffee machines from the database
        /// </summary>
        /// <returns>List of coffee machines</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        private async Task<List<Uri>> GetCoffeeMachinesAsync()
        {
            // Set up web request
            var coffeeMachines = await _context.Machines.Select(machine => new Uri(machine.CoffeeMachine)).ToListAsync();

            // Make sure that there are some machines configured
            if (coffeeMachines.Count == 0)
            {
                throw new NoCoffeeMachinesException(Resources.NoCoffeeMachines);
            }

            // Log known coffee machines
            var machineSb = new StringBuilder();
            foreach (var machine in coffeeMachines)
            {
                machineSb.Append(machine).Append(", ");
            }
            string machineList = machineSb.ToString();
            _logger.LogDebug("Configured machines are: {machines}.", machineList.TrimEnd().TrimEnd(','));

            return coffeeMachines;
        }

        /// <summary>
        /// Class to hold the order payload
        /// </summary>
        private class CoffeePayload
        {
            public Guid JobId { get; set; }
            public ECoffeeType Product { get; set; }
        }
    }
}
