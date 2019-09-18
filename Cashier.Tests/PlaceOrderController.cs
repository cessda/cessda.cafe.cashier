using Cashier.Contexts;
using Cashier.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cashier.Tests
{
    public class PlaceOrderController
    {
        private readonly CoffeeDbContext _context;
        private readonly Controllers.PlaceOrderController _controller;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public PlaceOrderController()
        {
            _context = new CoffeeDbContext();
            _controller = new Controllers.PlaceOrderController(_context);
        }

        [Fact]
        public void GetOrder_ReturnsAnActionResult_WithAListOfOrders()
        {
            // Arrange
            foreach (var order in ExampleOrders())
            {
                _context.Add(order);
            }
            _context.SaveChanges();

            // Act
            var getOrder = _controller.GetOrders().Result.Value;

            // Should be a list of orders
            Assert.IsType<List<Order>>(getOrder);

            // Should be a COFFEE_WITH_MILK
            Assert.Equal(ExampleOrders()[0].Coffees.ToList()[0].Product, getOrder[0].Coffees.ToList()[0].Product);
        }

        [Fact]
        public async void PostOrder_ReturnsCreatedOrder_ForAPostedOrder()
        {
            // Act
            var postOrder = await _controller.PostOrder(ExampleOrders()[0]);

            // Should be an IActionResult
            Assert.IsType<CreatedAtActionResult>(postOrder);
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
            var id = new Guid();

            // Act
            var deleteOrder = await _controller.DeleteOrder(id);

            // Should be NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(deleteOrder);

            // Should contain id
            Assert.Contains(id.ToString(), ((ApiMessage)deleteOrder).Message);
        }

        private static List<Order> ExampleOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    Coffees = new List<Coffee>()
                    {
                        new Coffee()
                        {
                            JobId = new Guid(),
                            Product = ECoffeeTypes.COFFEE_WITH_MILK,
                            OrderSize = 1
                        }
                    },
                    OrderId = new Guid(),
                    OrderSize = 1
                }
            };
        }
    }
}
