using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Engine;
using Cashier.Exceptions;
using Cashier.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests.Controllers
{
    public class ProcessJobsControllerTest
    {
        private readonly CashierDbContext _context;
        private readonly ProcessJobsController _controller;
        private readonly Mock<ICoffeeMachineService> _mock = new Mock<ICoffeeMachineService>();
        private readonly ICoffeeMachineService _orderEngine;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public ProcessJobsControllerTest()
        {
            _context = new Setup().SetupDb(nameof(ProcessJobsControllerTest));
            _orderEngine = _mock.Object;
            _controller = new ProcessJobsController(_context, _orderEngine);

            // Arrange
            _context.AddRange(ExampleOrders());
            _context.SaveChanges();
        }

        [Fact]
        public async Task PostCoffee_ReturnsAnApiMessage()
        {
            // Arrange
            int processedCoffees = _context.Jobs.Count(c => !string.IsNullOrEmpty(c.Machine));
            int queuedCoffees = _context.Jobs.Count(c => string.IsNullOrEmpty(c.Machine));
            _mock.Setup(o => o.StartAllJobsAsync());

            // Act
            var actionResult = await _controller.PostCoffee();

            // Should be OK
            var result = actionResult.Result as OkObjectResult;

            // Should contain a message
            var apiMessage = result.Value as ApiMessage;

            // StartCoffee should be called
            _mock.Verify(o => o.StartAllJobsAsync(), Times.AtLeastOnce);

            // Should be a list of orders
            Assert.NotNull(apiMessage);
            Assert.Contains(queuedCoffees.ToString(), apiMessage.Message);
            Assert.Contains(processedCoffees.ToString(), apiMessage.Message);
        }

        [Fact]
        public async Task PostCoffee_ShouldHandleException()
        {
            // Arrange
            _mock.Setup(o => o.StartAllJobsAsync()).Throws(new NoCoffeeMachinesException());

            // Act
            var actionResult = await _controller.PostCoffee();

            // Should be OK
            var result = actionResult.Result as ObjectResult;

            // Should be StatusCode 500
            Assert.Equal(500, result.StatusCode);

            // Should contain a message
            _ = result.Value as ApiMessage;
        }
    }
}
