using Cashier.Models.Database;
using System.Collections.Generic;

namespace Cashier.Models
{
    /// <summary>
    /// The model of the request submitted.
    /// </summary>
    public class CoffeeRequest
    {
        /// <summary>
        /// List of coffees in the order.
        /// </summary>
        public List<Coffee> Coffees { get; set; }

        /// <summary>
        /// Holds the type and amount of coffees ordered.
        /// </summary>
        public class Coffee
        {
            /// <summary>
            /// The product specified in the job.
            /// </summary>
            public ECoffeeType Product { get; set; }

            /// <summary>
            /// The amount of coffees to order
            /// </summary>
            public int Count { get; set; }
        }
    }
}
