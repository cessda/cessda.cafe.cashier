﻿using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Middleware;
using Cessda.Cafe.Cashier.Models.Database;
using Cessda.Cafe.Cashier.Service;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier
{
#pragma warning disable CS1591
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service object to configure</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(jsonOptions =>
            {
                // Convert enums into the strings representing them
                jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddRazorPages();

            // Set up the database
            services.AddDbContext<CashierDbContext>(options => options.UseInMemoryDatabase("coffee-db"));

            // Set up health checks
            services.AddHealthChecks();

            // Set up Swagger
            services.AddSwaggerDocument(swagger =>
            {
                swagger.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "CESSDA Café: Cashier API";
                };
            });

            // Set up engine
            services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();

            // Set up HTTP Client for the order engine
            services.AddHttpClient<ICoffeeMachineService, CoffeeMachineService>();

            // Set up correlation ID
            services.AddCorrelationId(options => new CorrelationIdOptions
            {
                RequestHeader = "X-Request-Id",
                ResponseHeader = "X-Request-Id",
                IncludeInResponse = true,
                CorrelationIdGenerator = new Func<string>(() => Guid.NewGuid().ToString()),
                UpdateTraceIdentifier = true
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <param name="env">Hosting Environment</param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Prometheus
            app.Map("/metrics", metricsApp =>
            {
                Metrics.SuppressDefaultMetrics();
                metricsApp.UseQueueLengthMetricsMiddleware();
                metricsApp.UseMetricServer("");
            });

            // Request ID
            app.UseCorrelationId();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Register the Swagger generator and the Swagger UI middleware
            app.UseOpenApi();
            app.UseSwaggerUi3();

            // Set up application routing
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions()
                {
                    ResponseWriter = WriteResponse
                });
            });
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            JObject json;

            if (result.Status == HealthStatus.Healthy)
            {
                // Healthy
                json = new JObject(new JProperty("message", "Ok"));
            }
            else
            {
                // Unhealthy
                json = new JObject(new JProperty("message", result.Status.ToString()));
            }
            return httpContext.Response.WriteAsync(json.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters", Justification = "Logger")]
        public static void ConfigureCoffeeMachines(IWebHost host)
        {
            // Validate parameters
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            using var scope = host.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CashierDbContext>();

            // If the environment specifies a coffee machine use it, else default to localhost
            var coffeeMachines = scope.ServiceProvider.GetRequiredService<IConfiguration>()
                .GetSection("Cafe:DefaultCoffeeMachine").AsEnumerable();
            var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();

            foreach (var coffeeMachine in coffeeMachines)
            {
                // Validate the Uri
                if (Uri.IsWellFormedUriString(coffeeMachine.Value, UriKind.Absolute))
                {
                    context.Machines.Add(new Machine() { CoffeeMachine = coffeeMachine.Value });
                    logger.LogInformation("Using coffee machine " + coffeeMachine.Value + ".");
                }
                else
                {
                    if (!string.IsNullOrEmpty(coffeeMachine.Value))
                    {
                        logger.LogWarning(coffeeMachine.Value + " is not a valid URI, not configuring.");
                    }
                }
            }

            context.SaveChanges();

            // If no coffee machines were added add the localhost coffee machine
            if (context.Machines.CountAsync().Result == 0)
            {
                const string defaultCoffeeMachine = "http://localhost:1337/";
                context.Machines.Add(new Machine() { CoffeeMachine = defaultCoffeeMachine });
                logger.LogInformation("Using default coffee machine " + defaultCoffeeMachine + ".");
                context.SaveChanges();
            }
        }
    }
}
