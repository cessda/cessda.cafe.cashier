using Cashier.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public CoffeeDbContext()
        {
            // Configure Carsten's Coffeepot by default
            if (Machines.Count() == 0)
            {
                Machines.Add(new Machines() { CoffeeMachine = "http://cafe-coffeepot:1337/" });
                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(_inMemDatabase);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        private const string _inMemDatabase = "coffee-db";
    }
}
