using System.ComponentModel.DataAnnotations;


namespace CESSDA.Cafe.Cashier.Models.Database
{
    /// <summary>
    /// Class to store the list of known machines.
    /// </summary>
    public class Machine
    {
        /// <summary>
        /// A URL of a coffee machine
        /// </summary>
        [Key]
        public string CoffeeMachine { get; set; }

        /// <summary>
        /// Construct a new instance of a <see cref="Machine"/> with the given coffee machine.
        /// </summary>
        /// <param name="coffeeMachine">The coffee machine.</param>
        public Machine(string coffeeMachine) => CoffeeMachine = coffeeMachine;
    }
}
