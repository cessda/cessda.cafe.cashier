using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Cashier.Models;

namespace Cashier.Contexts
{
    /// <summary>
    /// Defines the database context and options
    /// </summary>
    public class CoffeeDbContext : DbContext
    {
#pragma warning disable CS1591
        public DbSet<Order> Orders { get; set; }
        public DbSet<Coffee> Coffees { get; set; }
        public DbSet<Machines> Machines { get; set; }

        public CoffeeDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(_inMemDatabase);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        private readonly string _inMemDatabase = "coffee-db";
    }
}
