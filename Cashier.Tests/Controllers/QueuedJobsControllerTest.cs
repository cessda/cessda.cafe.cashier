using Cashier.Contexts;
using Cashier.Controllers;
using Cashier.Models;
using Cashier.Models.Database;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Cashier.Tests.TestData;

namespace Cashier.Tests.Controllers
{
    public class QueuedJobsControllerTest
    {
        private readonly CashierDbContext _context;
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
        public async Task GetCoffees_ReturnsAnActionResult_WithAListOfCoffees()
        {
            // Act
            var coffees = await _controller.GetCoffees();

            // Should be a list of orders
            Assert.IsType<CoffeeCount>(coffees.Value);

            // Should have a count greater than one
            Assert.True(coffees.Value.Count > 0);

            // All jobs should be queued
            foreach (var job in coffees.Value.Coffees)
            {
                Assert.Null(job.Machine);
            }
        }

        [Fact]
        public async Task GetCoffee_ReturnsAnActionResult_WithACoffee()
        {
            // Arrange
            var id = _context.Jobs
                .Where(j => string.IsNullOrEmpty(j.Machine))
                .First().JobId;

            // Act
            var job = await _controller.GetCoffee(id);

            // Should be a job
            Assert.IsType<Job>(job.Value);

            // Should have the same id
            Assert.Equal(id, job.Value.JobId);

            // Should be queued
            Assert.Null(job.Value.Machine);
        }
    }
}
