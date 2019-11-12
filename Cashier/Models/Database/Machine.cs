using System.ComponentModel.DataAnnotations;


namespace Cashier.Models.Database
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
    }
}
