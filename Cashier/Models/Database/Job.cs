using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CESSDA.Cafe.Cashier.Models.Database
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
        /// Creates a new Job object with the specified coffee machine. This is for test purposes.
        /// </summary>
        /// <param name="machine"></param>
        public Job(Uri machine)
        {
            ValidateMachine(machine);
            OrderPlaced = DateTime.Now;
            Machine = machine.ToString();
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
        public DateTime? JobStarted { get; private set; } = null;

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
        public string? Machine { get; private set; }

        /// <summary>
        /// The product specified in the job.
        /// </summary>
        [Required]
        public ECoffeeType Product { get; set; }

        /// <summary>
        /// Sets the job as started, indicating it has been sent to a coffee machine.
        /// If the job has already been sent this method does nothing.
        /// </summary>
        public void SetJobStarted()
        {
            if (JobStarted == null)
            {
                JobStarted = DateTime.Now;
            }
        }

        /// <summary>
        /// Set the coffee machine to the string form of the given Uri.
        /// If the machine is already set this method does nothing.
        /// </summary>
        public void SetMachine(Uri machine)
        {
            ValidateMachine(machine);
            if (string.IsNullOrEmpty(Machine))
            {
                Machine = machine.ToString();
            }
        }

        private static void ValidateMachine(Uri machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }
        }
    }
}
