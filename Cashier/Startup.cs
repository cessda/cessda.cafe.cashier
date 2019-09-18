using Cashier.Contexts;
using Cashier.Engine;
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
using System.Linq;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(jsonOptions =>
            {
                // Convert enums into the strings representing them
                jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
                jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }); ;

            // Set up the database
            services.AddDbContext<CoffeeDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemDatabase);
            });

            // Set up healthchecks
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
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder</param>
        /// <param name="env">Hosting Environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/healthcheck", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc();
        }


        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }

        private const string _inMemDatabase = "coffee-db";
    }
}
