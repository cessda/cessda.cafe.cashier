using Cashier.Contexts;
using Cashier.Models.Database;
using Microsoft.EntityFrameworkCore;
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
            var options = new DbContextOptionsBuilder<CoffeeDbContext>()
                .UseInMemoryDatabase(nameof(GetOrderController))
                .EnableSensitiveDataLogging(true)
                .Options;
            _context = new CoffeeDbContext(options);
            _controller = new Controllers.GetOrderController(_context);
        }

        [Fact]
        public void GetOrders_ReturnsAnActionResult_WithAListOfOrders()
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
    }
}
