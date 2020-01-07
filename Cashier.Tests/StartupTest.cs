using Cashier.Engine;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cashier.Tests
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureServices_ShouldSetupServices()
        {
            var webHost = Program.CreateWebHostBuilder(new string[0]);
            Assert.NotNull(webHost);
            Assert.NotNull(webHost.Services.GetRequiredService<ICoffeeMachineService>());
        }
    }
}
