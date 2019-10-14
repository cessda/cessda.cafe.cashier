using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashier.Models.Database
{
    /// <summary>
    /// Defines a coffee according to https://bitbucket.org/cessda/cessda.cafe/src/master/index.md
    /// </summary>
    public class Job
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
        public ECoffeeType Product { get; set; }

        /// <summary>
        /// State of the coffee.
        /// </summary>
        public ECoffeeState State { get; set; } = ECoffeeState.QUEUED;
    }
}
