﻿using Cashier.Contexts;
using Cashier.Models;
using Cashier.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests
{
    public class GetOrderController
    {
        private readonly CoffeeDbContext _context;
        private readonly Controllers.GetOrderController _controller;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public GetOrderController()
        {
            _context = new Setup().SetupDb(nameof(GetOrderController));
            _controller = new Controllers.GetOrderController(_context);

            // Arrange
            _context.AddRange(ExampleOrders());
            _context.SaveChanges();
        }

        [Fact]
        public void GetOrders_ReturnsAnActionResult_WithAListOfOrders()
        {
            // Act
            var getOrders = _controller.GetOrders().Result.Value;

            // Should be a list of orders
            Assert.IsType<List<Order>>(getOrders);

            // Should be a COFFEE_WITH_MILK
            Assert.NotNull(getOrders);
        }

        [Fact]
        public void GetOrder_ReturnsAnActionResult_WithAnOrder()
        {
            // Arrange
            var id = _context.Orders.First().OrderId;

            // Act
            var getOrder = _controller.GetOrder(id).Result;

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
        public void GetOrder_ReturnsAMessage_OnInvalidOrder()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var getOrder = _controller.GetOrder(id).Result;

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
