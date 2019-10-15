using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Engine;
using Cashier.Exceptions;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests.Controllers
{
    public class ProcessJobsControllerTest
    {
        private readonly CoffeeDbContext _context;
        private readonly ProcessJobsController _controller;
        private readonly Mock<IOrderEngine> _mock = new Mock<IOrderEngine>();
        private readonly IOrderEngine _orderEngine;

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
        public async void PostCoffee_ReturnsAnApiMessage()
        {
            // Arrange
            var queuedCoffees = _context.Coffees.Count(c => c.State == ECoffeeState.QUEUED);
            var processedCoffees = _context.Coffees.Count(c => c.State == ECoffeeState.PROCESSED);
            _mock.Setup(o => o.StartCoffee(It.IsAny<Guid>()));

            // Act
            var actionResult = await _controller.PostCoffee();

            // Should be OK
            var result = actionResult.Result as OkObjectResult;

            // Should contain a message
            var apiMessage = result.Value as ApiMessage;

            // StartCoffee should be called
            _mock.Verify(o => o.StartCoffee(It.IsAny<Guid>()), Times.AtLeastOnce);

            // Should be a list of orders
            Assert.NotNull(apiMessage);
            Assert.Contains(queuedCoffees.ToString(), apiMessage.Message);
            Assert.Contains(processedCoffees.ToString(), apiMessage.Message);
        }

        [Fact]
        public async void PostCoffee_ShouldHandleException()
        {
            // Arrange
            _mock.Setup(o => o.StartCoffee(It.IsAny<Guid>())).Throws(new NoCoffeeMachinesException());

            // Act
            var actionResult = await _controller.PostCoffee();

            // Should be OK
            var result = actionResult.Result as ObjectResult;

            // Should be StatusCode 500
            Assert.Equal(500, result.StatusCode);

            // Should contain a message
            var apiMessage = result.Value as ApiMessage;
        }
    }
}
