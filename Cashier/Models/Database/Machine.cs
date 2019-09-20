using System;
using System.ComponentModel.DataAnnotations;


namespace Cashier.Models.Database
{
    /// <summary>
    /// Class to store the list of known machines.
    /// </summary>
    public class Machine
    {
#pragma warning disable CS1591
        [Key]
        public string CoffeeMachine { get; set; }
    }
}
