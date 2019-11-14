using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Models;
using Cashier.Models.Database;
using System.Linq;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests.Controllers
{
    public class QueuedJobsControllerTest
    {
        private readonly CoffeeDbContext _context;
        private readonly QueuedJobsController _controller;

        /// <summary>
        /// Constructor, used to set up tests
        /// </summary>
        public QueuedJobsControllerTest()
        {
            _context = new Setup().SetupDb(nameof(QueuedJobsControllerTest));
            _controller = new QueuedJobsController(_context);

            // Arrange
            _context.AddRange(ExampleOrders());
            _context.SaveChanges();
        }

        [Fact]
        public void GetCoffees_ReturnsAnActionResult_WithAListOfCoffees()
        {
            // Act
            var coffees = _controller.GetCoffees().Result.Value;

            // Should be a list of orders
            Assert.IsType<CoffeeCount>(coffees);

            // Should have a count greater than one
            Assert.True(coffees.Count > 0);

            // All jobs should be queued
            foreach (var job in coffees.Coffees)
            {
                Assert.Null(job.Machine);
            }
        }

        [Fact]
        public void GetCoffee_ReturnsAnActionResult_WithACoffee()
        {
            // Arrange
            var id = _context.Jobs
                .Where(j => string.IsNullOrEmpty(j.Machine))
                .First().JobId;

            // Act
            var job = _controller.GetCoffee(id).Result.Value;

            // Should be a job
            Assert.IsType<Job>(job);

            // Should have the same id
            Assert.Equal(id, job.JobId);

            // Should be queued
            Assert.Null(job.Machine);
        }
    }
}
