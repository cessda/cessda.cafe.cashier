using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests.Controllers
{
    public class PlaceOrderControllerTest
    {
        private readonly CoffeeDbContext _context;
        private readonly PlaceOrderController _controller;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public PlaceOrderControllerTest()
        {
            _context = new Setup().SetupDb(nameof(PlaceOrderControllerTest));
            _controller = new PlaceOrderController(_context);
        }

        [Fact]
        public async void PostOrder_ReturnsCreatedOrder_ForAPostedOrder()
        {
            // Act
            var postOrder = await _controller.PostOrder(ExampleRequest());

            // Should be an IActionResult
            Assert.IsType<CreatedAtRouteResult>(postOrder);
            Assert.NotNull(postOrder);
            Assert.NotEmpty(_context.Orders);
        }

        [Fact]
        public async void DeleteOrder_ReturnsDeletedOrder_OnOrderDeletion()
        {
            // Arrange
            foreach (var order in ExampleOrders())
            {
                _context.Add(order);
            }
            _context.SaveChanges();

            // Make a copy of the id
            Guid id = _context.Orders.First().OrderId;

            // Act
            var deleteOrder = await _controller.DeleteOrder(id);

            // Should be OK
            Assert.IsType<OkObjectResult>(deleteOrder);

            // Order should be deleted
            Assert.Null(_context.Find<Order>(id));
        }

        [Fact]
        public async void DeleteOrder_ReturnsMessage_OnNullOrder()
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