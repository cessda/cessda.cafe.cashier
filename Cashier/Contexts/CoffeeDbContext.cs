using Cessda.Cafe.Cashier.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Cessda.Cafe.Cashier.Contexts
{
    /// <summary>
    /// Defines the database context and options
    /// </summary>
    public class CashierDbContext : DbContext
    {
#pragma warning disable CS1591
        public DbSet<Order> Orders { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public CashierDbContext(DbContextOptions<CashierDbContext> options) : base(options) { }
    }
}
