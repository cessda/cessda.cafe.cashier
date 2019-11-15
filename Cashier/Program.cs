using Cashier.Contexts;
using Cashier.Models.Database;
using Gelf.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Cashier
{
#pragma warning disable CS1591
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args);

            using (var scope = host.Services.CreateScope())
            using (var context = scope.ServiceProvider.GetService<CashierDbContext>())
            {
                context.Database.EnsureCreated();

                // If the environment specifies a coffee machine use it, else default to localhost
                var defaultCoffeeMachine = scope.ServiceProvider.GetService<IConfiguration>()["Cafe:DefaultCoffeeMachine"];
                if (string.IsNullOrEmpty(defaultCoffeeMachine))
                {
                    context.Machines.Add(new Machine() { CoffeeMachine = "http://localhost:1337/" });
                }
                else
                {
                    context.Machines.Add(new Machine() { CoffeeMachine = defaultCoffeeMachine });
                }
                context.SaveChanges();
            }

            host.Run();
            host.Dispose();
        }

        public static IWebHost CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Only configure GELF if a host is specified
                    if (!string.IsNullOrEmpty(hostingContext.Configuration["Logging:GELF:Host"]))
                    {
                        logging.AddGelf(options =>
                        {
                            options.LogSource = Environment.MachineName;
                            options.AdditionalFields["app_version"] = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                            options.AdditionalFields["cessda_component"] = hostingContext.HostingEnvironment.ApplicationName;
                            options.AdditionalFields["cessda_product"] = "CESSDA Café";
                        });
                    }
                })
                .UseStartup<Startup>()
                .Build();
    }
}
