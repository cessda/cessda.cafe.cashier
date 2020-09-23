using CESSDA.Cafe.Cashier.Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CESSDA.Cafe.Cashier.Models
{
    /// <summary>
    /// Class that counts the coffees that it holds.
    /// </summary>
    public class CoffeeCount
    {
        /// <summary>
        /// Counts the amounts of coffees held by this class.
        /// </summary>
        public CoffeeCount(IEnumerable<Job> coffees)
        {
            if (coffees == null)
            {
                throw new ArgumentNullException(nameof(coffees));
            }
            Coffees = coffees.ToImmutableList();
            Count = Coffees.Count;
        }
        /// <summary>
        /// Amount of coffees held.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Coffees to be counted.
        /// </summary>
        public ImmutableList<Job> Coffees { get; }
    }
}
