using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Cashier.Engine
{
    /// <summary>
    /// Class to hold the logic of contacting coffee machines.
    /// </summary>
    public class OrderEngine : IOrderEngine
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for OrderEngine, used for passing the logger and the database context.
        /// </summary>
        /// <param name="context">Coffee Database Context</param>
        /// <param name="logger">Logger</param>
        public OrderEngine(CoffeeDbContext context, ILogger<OrderEngine> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Start the specified order
        /// </summary>
        /// <param name="id">The order to start</param>
        public void StartOrder(Guid id)
        {
            // Load the order
            var order = _context.Orders.Include(b => b.Coffees).Single(o => o.OrderId == id);
            _logger.LogInformation("Starting order: " + order.OrderId);

            // For each coffee
            foreach (var coffee in order.Coffees)
            {
                StartCoffee(coffee.JobId);
            }
        }

        /// <summary>
        /// Start the specifed coffee
        /// </summary>
        /// <param name="id">ID of coffee to start</param>
        public void StartCoffee(Guid id)
        {
            // Load the coffee
            var coffee = _context.Coffees.Single(c => c.JobId == id);

            bool success = false;

            // Check if the coffee has already been sent to a machine
            if (string.IsNullOrEmpty(coffee.Machine))
            {
                _logger.LogInformation("Starting coffee " + coffee.JobId + ".");

                // Set up web request
                var coffeeMachines = GetCoffeeMachines();

                var coffeePayload = new CoffeePayload()
                {
                    OrderId = coffee.OrderId,
                    OrderPlaced = coffee.OrderPlaced,
                    OrderSize = coffee.OrderSize,
                    Product = coffee.Product
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

                    success = SendRequest(json, machine);

                    if (success)
                    {
                        // Sucess - mark the time that the order was sent
                        _logger.LogInformation("Sent job " + coffee.OrderId + " to machine " + machine);
                        coffee.JobStarted = DateTime.Now;
                        coffee.Machine = machine.ToString();
                        coffee.State = ECoffeeState.PROCESSED;

                        // Update the database
                        _context.Entry(coffee).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                // If we didn't succeed
                if (!success)
                {
                    _logger.LogWarning("No coffee machines could accept job " + coffee.JobId + ".");
                }
            }
            else
            {
                _logger.LogWarning("Attempted to start coffee " + coffee.JobId + " which has already been started");
            }
        }

        private bool SendRequest(string json, Uri machine)
        {
            // Create a new WebRequest
            _logger.LogDebug("Using machine: " + machine + ".");
            var httpWebRequest = WebRequest.CreateHttp(new Uri(machine, "start-job"));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                // Submit the coffee to the coffee machine
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                // Read the response from the coffee machine
                try
                {
                    var httpResponse = httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var response = JsonConvert.DeserializeObject<Job>(streamReader.ReadToEnd());
                        _logger.LogDebug("Response from " + machine + ": " + response);
                    }

                    return true;
                }
                catch (WebException e)
                {
                    if (e.Response == null) throw;
                    using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        // TODO - Change behavior based on message
                        var stringResponse = streamReader.ReadToEnd();
                        var responseCode = ((HttpWebResponse)e.Response).StatusCode;
                        try
                        {
                            var response = JsonConvert.DeserializeObject<ApiMessage>(stringResponse);
                            _logger.LogWarning("Coffee machine " + machine + " responded with code " + (int)responseCode + " and with message: " + response.Message);
                            return false;
                        }
                        catch (JsonSerializationException)
                        {
                            _logger.LogWarning("Coffee machine " + machine + " responded with code " + (int)responseCode + ". The message could not be parsed.");
                            return false;
                        }
                    }
                }
            }
            // If the message is null (i.e. connection issue)
            catch (WebException e)
            {
                _logger.LogError("Connecting to coffee machine " + machine.Host + " failed: " + e.Message);
                return false;
            }
        }

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
                throw new Exceptions.NoCoffeeMachinesException("No coffee machines have been configured.");
            }

            // Log known coffee machines
            var machineSb = new StringBuilder();
            foreach (var machine in coffeeMachines)
            {
                machineSb.Append(machine + ", ");
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
            public Guid OrderId { get; set; }
            public DateTime OrderPlaced { get; set; }
            public int OrderSize { get; set; }
            public ECoffeeTypes Product { get; set; }
        }
    }
}
