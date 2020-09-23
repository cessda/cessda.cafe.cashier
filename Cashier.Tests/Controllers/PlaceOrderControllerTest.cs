using CESSDA.Cafe.Cashier.Contexts;
using CESSDA.Cafe.Cashier.Controllers;
using CESSDA.Cafe.Cashier.Models;
using CESSDA.Cafe.Cashier.Models.Database;
using CESSDA.Cafe.Cashier.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CESSDA.Cafe.Cashier.Tests.Controllers
{
    public class PlaceOrderControllerTest
    {
        private readonly CashierDbContext _context;
        private readonly PlaceOrderController _controller;
        private readonly Mock<ICoffeeMachineService> _mock = new Mock<ICoffeeMachineService>();
        private readonly ICoffeeMachineService _orderEngine;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public PlaceOrderControllerTest()
        {
            _context = new Setup().SetupDb(nameof(PlaceOrderControllerTest));
            _orderEngine = _mock.Object;
            _controller = new PlaceOrderController(_context, Mock.Of<ILogger<PlaceOrderController>>(), _orderEngine);
        }

        [Fact]
        public async Task PostOrder_ReturnsCreatedOrder_ForAPostedOrder()
        {
            // Act
            var postOrder = await _controller.PostOrder(TestData.ExampleRequest());

            // Should be an IActionResult
            Assert.IsType<CreatedAtRouteResult>(postOrder);
            Assert.NotNull(postOrder);
            Assert.NotEmpty(_context.Orders);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsDeletedOrder_OnOrderDeletion()
        {
            // Arrange
            foreach (var order in TestData.ExampleDeletableOrders())
            {
                _context.Add(order);
            }
            _context.SaveChanges();

            // Make a copy of the id
            var id = _context.Orders.First().OrderId;

            // Act
            var deleteOrder = await _controller.DeleteOrder(id);

            // Should be OK
            Assert.IsType<OkObjectResult>(deleteOrder);

            // Order should be deleted
            Assert.Null(_context.Find<Order>(id));
        }

        [Fact]
        public async Task DeleteOrder_ReturnsMessage_OnNullOrder()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var deleteOrder = await _controller.DeleteOrder(id);

            // Should be NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(deleteOrder);
            var result = deleteOrder as NotFoundObjectResult;
            var value = result.Value as ApiMessage;

            // Should contain id
            Assert.Contains(id.ToString(), value.Message);
        }
    }
}
