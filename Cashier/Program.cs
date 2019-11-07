using Cashier.Contexts;
using Cashier.Models.Database;
using Gelf.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            using (var context = scope.ServiceProvider.GetService<CoffeeDbContext>())
            {
                // Configure Carsten's Coffeepot by default
                context.Database.EnsureCreated();
                context.Machines.Add(new Machine() { CoffeeMachine = "http://cafe-coffee-carsten:1337/" });
                context.SaveChanges();
            }

            host.Run();
            host.Dispose();
        }

        public static IWebHost CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddGelf(options =>
                    {
                        if (string.IsNullOrEmpty(options.Host)) options.Host = "localhost";
                        options.LogSource = hostingContext.HostingEnvironment.ApplicationName;
                        options.AdditionalFields["app_version"] = Assembly.GetEntryAssembly()
                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                        options.AdditionalFields["machine_name"] = Environment.MachineName;
                        options.AdditionalFields["project_name"] = "CESSDA Café";
                    });
                })
                .UseStartup<Startup>()
                .Build();
    }
}
