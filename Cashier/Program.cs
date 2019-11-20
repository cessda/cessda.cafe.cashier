using Cashier.Contexts;
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
            using (var context = scope.ServiceProvider.GetService<CashierDbContext>())
            {
                context.Database.EnsureCreated();
            }

            Startup.ConfigureCoffeeMachines(host);

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
