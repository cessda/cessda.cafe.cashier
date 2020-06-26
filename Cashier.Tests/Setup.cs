using Cessda.Cafe.Cashier.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Cessda.Cafe.Cashier.Tests
{
    internal class Setup
    {
        internal CashierDbContext SetupDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<CashierDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging(true)
                .Options;

            // Create the context
            var context = new CashierDbContext(options);

            // Ensure the database is created
            context.Database.EnsureCreated();
            return context;
        }
    }
}
