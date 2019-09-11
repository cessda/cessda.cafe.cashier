using Cashier.Contexts;
using Cashier.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace Cashier.Engine
{
    
    public class OrderEngine
    {
        private readonly CoffeeDbContext _context;
        private readonly ILogger _logger;

        public OrderEngine(CoffeeDbContext context, ILogger logger)
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
            // Set up web request
            // TODO: Store globally
            var machines = new Machines() { CoffeeMachines = new List<string>() { "http://localhost:1337" } };

            // Make sure that there are some machines configured
            if (machines.CoffeeMachines.Count == 0)
            {
                throw new Exceptions.NoCoffeeMachinesException("No coffee machines have been configured.");
            }

            // Set up the JSON converter
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new StringEnumConverter());
            jsonSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Load the order
            var order = _context.Orders.Include(b => b.Coffees).Single(o => o.OrderId == id);

            // For each coffee
            foreach (var coffee in order.Coffees)
            {
                bool success = false;

                var coffeePayload = new CoffeePayload();

                coffeePayload.OrderId = coffee.OrderId;
                coffeePayload.OrderPlaced = coffee.OrderPlaced;
                coffeePayload.OrderSize = coffee.OrderSize;
                coffeePayload.Product = coffee.Product;

                var json = JsonConvert.SerializeObject(coffeePayload, jsonSettings);
                _logger.LogDebug(json);

                // Iterate through all known coffee machines
                foreach (var machine in machines.CoffeeMachines)
                {
                    // Break if the order has already been sent to a machine
                    if (success) break;

                    try
                    {
                        // Create a new WebRequest
                        var httpWebRequest = WebRequest.CreateHttp(machine + "/start-job");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

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
                                var response = streamReader.ReadToEnd();
                                var jsonResponse = JsonConvert.DeserializeObject<Coffee>(response);
                            }

                            // Sucess - mark the time that the order was sent
                            _logger.LogInformation("Sent order " + coffeePayload.OrderId + " to machine " + machine);
                            coffee.JobStarted = DateTime.Now;
                            coffee.State = ECoffeeState.PROCESSING;
                            success = true;
                        }
                        catch (WebException e)
                        {
                            if (e.Response == null) throw;
                            using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
                            {
                                // TODO - Change behavior based on message
                                var response = streamReader.ReadToEnd().ToString();
                                var responseCode = ((HttpWebResponse)e.Response).StatusCode;
                                var parsedResponse = JsonConvert.DeserializeObject<ApiMessage>(response);

                                _logger.LogWarning("Coffee machine " + machine + " responded with code " + (int)responseCode + " and with message:" + parsedResponse.Message);
                            }
                        }
                    }
                    // If the message is null (i.e. connection issue)
                    catch (WebException e)
                    {
                        _logger.LogWarning(machine + ": " + e.Message);
                    }
                }

                // If we didn't succeed
                if (!success)
                {
                    _logger.LogError("No coffee machines could accept the request.");
                }
            }
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
        /// <summary>
        /// Class to send and hold messages from other coffee APIs
        /// </summary>
        private class ApiMessage
        {
            public string Message { get; set; }
        }
    }
}
