using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Cashier.Properties;
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

namespace Cashier.Engine
{
    /// <summary>
    /// Class to hold the logic of contacting coffee machines.
    /// </summary>
    public class OrderEngine : IOrderEngine
    {
        private readonly CoffeeDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for OrderEngine, used for passing the logger and the database context.
        /// </summary>
        /// <param name="context">Coffee Database Context</param>
        /// <param name="httpClient">HTTP Client</param>
        /// <param name="logger">Logger</param>
        public OrderEngine(CoffeeDbContext context, HttpClient httpClient, ILogger<OrderEngine> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Starts the jobs associated with an order.
        /// </summary>
        /// <param name="id">The order to start.</param>
        public async Task StartOrderAsync(Guid id)
        {
            var order = await _context.Orders.Include(b => b.Jobs)
                .SingleAsync(o => o.OrderId == id).ConfigureAwait(true);

            await StartJobListAsync(order.Jobs).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts all jobs that haven't been started.
        /// </summary>
        public async Task StartAllJobsAsync()
        {
            var jobs = await _context.Jobs.ToListAsync().ConfigureAwait(true);

            await StartJobListAsync(jobs).ConfigureAwait(false);
        }

        private async Task StartJobListAsync(List<Job> jobs)
        {
            var taskList = new List<Task>();

            foreach (var job in jobs)
            {
                taskList.Add(StartJobAsync(job.JobId));
            }

            await Task.WhenAll(taskList).ConfigureAwait(false);
        }

        /// <summary>
        /// Start the specified coffee
        /// </summary>
        /// <param name="id">ID of coffee to start</param>
        public async Task StartJobAsync(Guid id)
        {
            // Load the coffee
            var job = _context.Jobs.Single(c => c.JobId == id);

            // Check if the coffee has already been sent to a machine
            if (string.IsNullOrEmpty(job.Machine))
            {
                _logger.LogInformation("Starting job " + job.JobId + ".");

                // Set up web request
                bool success = false;
                var coffeeMachines = GetCoffeeMachines();

                var coffeePayload = new CoffeePayload()
                {
                    JobId = job.JobId,
                    Product = job.Product
                };

                // Set up the JSON converter
                var jsonSettings = new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                jsonSettings.Converters.Add(new StringEnumConverter());

                var json = JsonConvert.SerializeObject(coffeePayload, jsonSettings);
                _logger.LogDebug("Created JSON: " + json);

                // Iterate through all known coffee machines
                foreach (var machine in coffeeMachines)
                {
                    // Break if the order has already been sent to a machine
                    if (success) break;
                    success = await SendRequestAsync(json, machine).ConfigureAwait(true);

                    // Update local state to mark that the coffee was sent to a remote machine
                    if (success)
                    {
                        // Mark the time that the order was sent
                        _logger.LogInformation("Sent job " + job.JobId + " to machine " + machine);
                        job.JobStarted = DateTime.Now;
                        job.Machine = machine.ToString();

                        // Update the database
                        _context.Entry(job).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                // No coffee machines could accept the coffee
                if (!success)
                {
                    _logger.LogWarning("No coffee machines could accept job " + job.JobId + ".");
                }
            }
            else
            {
                _logger.LogWarning("Attempted to start coffee " + job.JobId + " which has already been started");
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
            _logger.LogDebug("Using machine: " + uri + ".");
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Submit the coffee to the coffee machine
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(new Uri(uri, "start-job"), content).ConfigureAwait(true);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response from the coffee machine
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    JsonConvert.DeserializeObject<Job>(responseString);
                    _logger.LogDebug("Response from " + uri + ": " + responseString);
                    return true;
                }
                else
                {
                    var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    var responseCode = response.StatusCode;
                    try
                    {
                        var apiMessage = JsonConvert.DeserializeObject<ApiMessage>(stringResponse);
                        if (stringResponse == "Machine busy!")
                        {
                            _logger.LogInformation("Coffee machine" + uri + "is busy.");
                        }
                        else
                        {
                            _logger.LogWarning("Coffee machine " + uri + " responded with code " + (int)responseCode + " and with message: " + apiMessage.Message);
                        }
                    }
#pragma warning disable CA1031
                    catch (JsonSerializationException)
                    {
                        _logger.LogWarning("Coffee machine " + uri + " responded with code " + (int)responseCode + ". The message could not be parsed.");
                    }
#pragma warning restore CA1031
                    return false;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Connecting to coffee machine " + uri + " failed: " + e.Message);
                return false;
            }
            finally
            {
                content.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of known coffee machines from the database
        /// </summary>
        /// <returns>List of coffee machines</returns>
        private List<Uri> GetCoffeeMachines()
        {
            // Set up web request
            var coffeeMachines = new List<Uri>();
            foreach (var machine in _context.Machines.ToList())
            {
                coffeeMachines.Add(new Uri(machine.CoffeeMachine));
            }

            // Make sure that there are some machines configured
            if (coffeeMachines.Count == 0)
            {
                throw new Exceptions.NoCoffeeMachinesException(Resources.NoCoffeeMachines);
            }

            // Log known coffee machines
            var machineSb = new StringBuilder();
            foreach (var machine in coffeeMachines)
            {
                machineSb.Append(machine).Append(", ");
            }
            string machineList = machineSb.ToString();
            _logger.LogDebug("Configured machines are: " + machineList.TrimEnd().TrimEnd(",".ToCharArray()));

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
