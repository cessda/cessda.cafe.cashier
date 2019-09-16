using System;
using System.ComponentModel.DataAnnotations;


namespace Cashier.Models
{
    /// <summary>
    /// Class to store the list of known machines.
    /// </summary>
    public class Machines
    {
#pragma warning disable CS1591
        [Key]
        public string CoffeeMachine { get; set; }
    }
}
