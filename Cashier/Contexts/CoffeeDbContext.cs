using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Cashier.Models;

namespace Cashier.Contexts
{
    public class CoffeeDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Coffee> Coffees { get; set; }
        public DbSet<Machines> Machines { get; set; }

        public CoffeeDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase(_inMemDatabase);
            options.EnableSensitiveDataLogging(true);
        }

        private readonly string _inMemDatabase = "coffee-db";
    }
}
