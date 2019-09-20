using Cashier.Models.Database;
using System.Collections.Generic;

namespace Cashier.Models
{
    /// <summary>
    /// Class that counts the coffees that it holds.
    /// </summary>
    public class CoffeeCount
    {
        /// <summary>
        /// Counts the amounts of coffees held by this class.
        /// </summary>
        public CoffeeCount(List<Job> coffees)
        {
            Count = coffees.Count;
            Coffees = coffees;
        }
        /// <summary>
        /// Amount of coffees held.
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Coffees to be counted.
        /// </summary>
        public virtual ICollection<Job> Coffees { get; set; }
    }
}
