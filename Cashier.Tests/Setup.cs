using Cashier.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Cashier.Tests
{
    internal class Setup
    {
        internal CoffeeDbContext SetupDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<CoffeeDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging(true)
                .Options;

            // Create the context
            var context = new CoffeeDbContext(options);

            // Ensure the database is created
            context.Database.EnsureCreated();
            return context;
        }
    }
}
