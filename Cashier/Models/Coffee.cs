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
        COFFEE,
        STRONG_COFFEE,
        CAPPUCCINO,
        MOCCACHINO,
        COFFEE_WITH_MILK,
        ESPRESSO,
        ESPRESSO_CHOCOLATE,
        KAKAO,
        HOT_WATER
    }

    /// <summary>
    /// Coffee state
    /// </summary>
    public enum ECoffeeState : byte
    {
        QUEUED,
        PROCESSING,
        PROCESSED
    }

    public class CoffeeCount
    {

        public CoffeeCount(List<Coffee> coffees)
        {
            count = coffees.Count;
            Coffees = coffees;
        }

        public int count { get; private set; }
        public virtual ICollection<Coffee> Coffees { get; set; }
    }

    public class Coffee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobId { get; set; }
        public DateTime? JobStarted { get; set; } = null;

        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
        public DateTime OrderPlaced { get; set; } = DateTime.Now;

        [Required]
        public int OrderSize { get; set; }
        public string Machine { get; set; }

        [Required]
        public ECoffeeTypes Product { get; set; }
        public ECoffeeState State { get; set; } = ECoffeeState.QUEUED;
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderId { get; set; }
        public int OrderSize { get; set; }

        [Required]
        public virtual ICollection<Coffee> Coffees { get; set; }
    }
}
