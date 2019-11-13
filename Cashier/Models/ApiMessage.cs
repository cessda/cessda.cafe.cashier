using Cashier.Properties;
using System;
using System.Globalization;

namespace Cashier.Models
{
    /// <summary>
    /// Class to send and hold messages from other coffee APIs
    /// </summary>
    public class ApiMessage
    {
        /// <summary>
        /// Message string
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a message stating no coffees were found.
        /// </summary>
        /// <returns>The ApiMessage.</returns>
        public static ApiMessage NoCoffees()
        {
            return new ApiMessage() { Message = Resources.NoCoffees };
        }

        /// <summary>
        /// Creates a message stating the order was not found.
        /// </summary>
        /// <param name="id">The Order id not found.</param>
        /// <returns>The ApiMessage.</returns>
        public static ApiMessage OrderNotFound(Guid id)
        {
            return new ApiMessage()
            {
                Message = string.Format(CultureInfo.InvariantCulture, Resources.OrderNotFound, id.ToString())
            };
        }
    }
}
