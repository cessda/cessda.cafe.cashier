using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
