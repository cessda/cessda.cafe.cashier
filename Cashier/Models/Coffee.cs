using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashier.Models
{
    /// <summary>
    /// List of allowed coffees
    /// </summary>
    public enum ECoffeeTypes : byte
    {
#pragma warning disable CS1591
        COFFEE,
        STRONG_COFFEE,
        CAPPUCCINO,
        MOCCACHINO,
        COFFEE_WITH_MILK,
        ESPRESSO,
        ESPRESSO_CHOCOLATE,
        KAKAO,
        HOT_WATER
#pragma warning restore CS1591
    }

    /// <summary>
    /// Coffee state
    /// </summary>
    public enum ECoffeeState : byte
    {
#pragma warning disable CS1591
        QUEUED,
        PROCESSED
#pragma warning restore CS1591
    }

    /// <summary>
    /// Class that counts the coffees that it holds.
    /// </summary>
    public class CoffeeCount
    {
        /// <summary>
        /// Counts the amounts of coffees held by this class.
        /// </summary>
        public CoffeeCount(List<Coffee> coffees)
        {
            count = coffees.Count;
            Coffees = coffees;
        }
        /// <summary>
        /// Amount of coffees held.
        /// </summary>
        public int count { get; private set; }
        /// <summary>
        /// Coffees to be counted.
        /// </summary>
        public virtual ICollection<Coffee> Coffees { get; set; }
    }

    /// <summary>
    /// Defines a coffee according to https://bitbucket.org/cessda/cessda.cafe/src/master/index.md
    /// </summary>
    public class Coffee
    {
        /// <summary>
        /// The ID of the job
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobId { get; set; }
        /// <summary>
        /// The time the job was started on a remote coffee machine.
        /// </summary>
        public DateTime? JobStarted { get; set; } = null;
        /// <summary>
        /// The order this job belongs to.
        /// </summary>
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
        /// <summary>
        /// The time the order was placed.
        /// </summary>
        public DateTime OrderPlaced { get; private set; } = DateTime.Now;
        /// <summary>
        /// The size of the order.
        /// </summary>
        [Required]
        public int OrderSize { get; set; }
        /// <summary>
        /// The coffee machine the order was run on.
        /// </summary>
        public string Machine { get; set; }
        /// <summary>
        /// The product specified in the job.
        /// </summary>
        [Required]
        public ECoffeeTypes Product { get; set; }
        /// <summary>
        /// State of the coffee.
        /// </summary>
        public ECoffeeState State { get; set; } = ECoffeeState.QUEUED;
    }

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
        /// The total amount of coffees present in the order.
        /// </summary>
        public int OrderSize { get; set; }
        /// <summary>
        /// Coffees associated with this order.
        /// </summary>
        [Required]
        public virtual ICollection<Coffee> Coffees { get; set; }
    }
}
