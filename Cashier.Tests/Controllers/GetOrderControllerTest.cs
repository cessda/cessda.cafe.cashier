using Cessda.Cafe.Cashier.Contexts;
using Cessda.Cafe.Cashier.Controllers;
using Cessda.Cafe.Cashier.Models;
using Cessda.Cafe.Cashier.Models.Database;
using Cessda.Cafe.Cashier.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cessda.Cafe.Cashier.Tests.Controllers
{
    public class GetOrderControllerTest
    {
        private readonly CashierDbContext _context;
        private readonly GetOrderController _controller;
        private readonly Mock<ICoffeeMachineService> _mock = new Mock<ICoffeeMachineService>();
        private readonly ICoffeeMachineService _orderEngine;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public GetOrderControllerTest()
        {
            _context = new Setup().SetupDb(nameof(GetOrderControllerTest));
            _orderEngine = _mock.Object;
            _controller = new GetOrderController(_context, _orderEngine);

            // Arrange
            _context.AddRange(TestData.ExampleOrders());
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetOrders_ReturnsAnActionResult_WithAListOfOrders()
        {
            // Act
            var getOrders = await _controller.GetOrders();

            // Should be a list of orders
            Assert.IsType<List<Order>>(getOrders.Value);

            // Should be a COFFEE_WITH_MILK
            Assert.NotNull(getOrders);
        }

        [Fact]
        public async Task GetOrder_ReturnsAnActionResult_WithAnOrder()
        {
            // Arrange
            var id = _context.Orders.First().OrderId;

            // Act
            var getOrder = await _controller.GetOrder(id);

            // Should be an OkObjectResult
            Assert.IsType<OkObjectResult>(getOrder);

            // Should contain an order
            var orderResult = (OkObjectResult)getOrder;
            Assert.IsType<Order>(orderResult.Value);

            // Should have the submitted ID
            var order = (Order)orderResult.Value;
            Assert.Equal(id, order.OrderId);
        }

        [Fact]
        public async Task GetOrder_ReturnsAMessage_OnInvalidOrder()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var getOrder = await _controller.GetOrder(id);

            // Should be a NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(getOrder);

            // Should contain an ApiMessage
            var orderResult = (NotFoundObjectResult)getOrder;
            Assert.IsType<ApiMessage>(orderResult.Value);

            // Should have the submitted ID
            var message = (ApiMessage)orderResult.Value;
            Assert.Contains(id.ToString(), message.Message);
        }
    }
}
