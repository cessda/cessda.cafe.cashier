using Cessda.Cafe.Cashier.Service;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cessda.Cafe.Cashier.Tests
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureServices_ShouldSetupServices()
        {
            var webHost = Program.CreateWebHostBuilder(System.Array.Empty<string>());
            Assert.NotNull(webHost);
            Assert.NotNull(webHost.Services.GetRequiredService<ICoffeeMachineService>());
        }
    }
}
