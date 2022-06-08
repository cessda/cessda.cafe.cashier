using CESSDA.Cafe.Cashier.Contexts;
using CESSDA.Cafe.Cashier.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http;

namespace CESSDA.Cafe.Cashier.Tests.Service
{
    internal class CoffeeMachineServiceTest
    {
        private readonly CashierDbContext _context;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly CoffeeMachineService _coffeeMachineService;

        CoffeeMachineServiceTest()
        {
            var factory = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();

            _context = new Setup().SetupDb(nameof(CoffeeMachineServiceTest));
            _mockHttpClient = new Mock<HttpClient>();
            _coffeeMachineService = new CoffeeMachineService(
                _context,
                _mockHttpClient.Object,
                factory.CreateLogger<CoffeeMachineService>(),
                null);
        }
    }
}
