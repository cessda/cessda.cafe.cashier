using Cashier.Contexts;
using Cashier.Engine;
using Cashier.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Cashier
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(jsonOptions =>
            {
                // Convert enums into the strings representing them
                jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            // Set up the database
            services.AddDbContext<CoffeeDbContext>(options => options.UseInMemoryDatabase(_inMemDatabase));

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
            services.AddScoped<IOrderEngine, OrderEngine>();

            // Set up HTTP Client for the order engine
            services.AddHttpClient<IOrderEngine, OrderEngine>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <param name="env">Hosting Environment</param>
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/healthcheck", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRequestIdMiddleware();

            // Register the Swagger generator and the Swagger UI middleware
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc();
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

        private const string _inMemDatabase = "coffee-db";
    }
}
