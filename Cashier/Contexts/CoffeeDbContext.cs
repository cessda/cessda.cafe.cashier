using Cashier.Models.Database;
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
        public DbSet<Job> Coffees { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options){}
    }
}
