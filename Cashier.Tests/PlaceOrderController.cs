using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cashier.Tests
{
    public class PlaceOrderController
    {
        private readonly CoffeeDbContext _context;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public PlaceOrderController()
        {
            _context = new CoffeeDbContext();
        }

        [Fact]
        public void GetOrder_ReturnsAnActionResult_WithAListOfOrders()
        {
            // Arrange
            var service = new Controllers.PlaceOrderController(_context);
            foreach (var order in ExampleOrders())
            {
                _context.Add(order);
            }
            _context.SaveChanges();

            // Act
            var getOrder = service.GetOrders().Result.Value;

            // Should be a list of orders
            Assert.IsType<List<Order>>(getOrder);

            // Should be a COFFEE_WITH_MILK
            Assert.Equal(ExampleOrders()[0].Coffees.ToList()[0].Product, getOrder[0].Coffees.ToList()[0].Product);
        }

        [Fact]
        public async void PostOrder_ReturnsCreatedOrder_ForAPostedOrder()
        {
            // Arrange
            var service = new Controllers.PlaceOrderController(_context);

            // Act
            var postOrder = await service.PostOrder(ExampleOrders()[0]);

            // Should be an IActionResult
            Assert.IsType<CreatedAtActionResult>(postOrder);
            Assert.NotNull(postOrder);
            Assert.NotEmpty(_context.Orders);
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
