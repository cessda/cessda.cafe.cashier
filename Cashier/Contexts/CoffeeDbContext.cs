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

        public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options)
        {
            // Configure Carsten's Coffeepot by default
            if (Machines.Count() == 0)
            {
                Machines.Add(new Machines() { CoffeeMachine = "http://cafe-coffeepot:1337/" });
                SaveChanges();
            }
        }
    }
}
