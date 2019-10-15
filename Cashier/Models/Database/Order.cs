using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashier.Models.Database
{
    /// <summary>
    /// Defines an order according to https://bitbucket.org/cessda/cessda.cafe/src/master/index.md
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The id of the order.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderId { get; set; }

        /// <summary>
        /// The time the order was placed.
        /// </summary>
        public DateTime OrderPlaced { get; private set; } = DateTime.Now;

        /// <summary>
        /// The total amount of coffees present in the order.
        /// </summary>
        public int OrderSize { get; set; }

        /// <summary>
        /// Coffees associated with this order.
        /// </summary>
        [Required]
        public virtual List<Job> Coffees { get; } = new List<Job>();
    }
}
