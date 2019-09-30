using Cashier.Contexts;
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
            using (var context = scope.ServiceProvider.GetService<CoffeeDbContext>())
            {
                context.Database.EnsureCreated();
            }

            host.Run();
        }

        public static IWebHost CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddGelf(options =>
                    {
                        options.Host = "localhost";
                        options.Protocol = GelfProtocol.Http;
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
