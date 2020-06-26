using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cessda.Cafe.Cashier.Models.Database
{
    /// <summary>
    /// Defines a coffee according to https://bitbucket.org/cessda/cessda.cafe/src/master/index.md
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Create a new Job object with the OrderPlaced set to the current time
        /// </summary>
        public Job()
        {
            OrderPlaced = DateTime.Now;
        }

        /// <summary>
        /// Create a new Job object with the OrderPlaced set to the specified time
        /// </summary>
        /// <param name="orderPlaced">The time the order was placed</param>
        public Job(DateTime orderPlaced)
        {
            OrderPlaced = orderPlaced;
        }

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
        [Required]
        public DateTime OrderPlaced { get; private set; }

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
    }
}
