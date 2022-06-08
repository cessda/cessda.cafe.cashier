using CESSDA.Cafe.Cashier.Contexts;
using CESSDA.Cafe.Cashier.Middleware;
using CESSDA.Cafe.Cashier.Models.Database;
using CESSDA.Cafe.Cashier.Service;
using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
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

namespace CESSDA.Cafe.Cashier
{
    /// <summary>
    /// Configures the cashier
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Key/Value application properties
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Injects configuration into the startup
        /// </summary>
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
            services.AddSingleton<ICoffeeMachineService, CoffeeMachineService>();

            // Set up HTTP Client for the order engine
            services.AddHttpClient<ICoffeeMachineService, CoffeeMachineService>().AddCorrelationIdForwarding();

            // Set up correlation ID
            services.AddDefaultCorrelationId(options =>
            {
                options.RequestHeader = "X-Request-Id";
                options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
                options.UpdateTraceIdentifier = true;
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

        internal static void ConfigureCoffeeMachines(IWebHost host)
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
                    context.Machines.Add(new Machine(coffeeMachine.Value));
                    logger!.LogInformation("Using coffee machine {coffeeMachine}.", coffeeMachine.Value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(coffeeMachine.Value))
                    {
                        logger!.LogWarning("{coffeeMachine} is not a valid URI, not configuring.", coffeeMachine.Value);
                    }
                }
            }

            context.SaveChanges();

            // If no coffee machines were added add the localhost coffee machine
            if (context.Machines.CountAsync().Result == 0)
            {
                const string defaultCoffeeMachine = "http://localhost:1337/";
                context.Machines.Add(new Machine(defaultCoffeeMachine));
                logger!.LogInformation("Using default coffee machine {defaultCoffeeMachine}.", defaultCoffeeMachine);
                context.SaveChanges();
            }
        }
    }
}
