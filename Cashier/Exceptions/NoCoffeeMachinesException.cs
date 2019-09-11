using System;
using System.Runtime.Serialization;

namespace Cashier.Exceptions
{
    [Serializable]
    internal class NoCoffeeMachinesException : Exception
    {
        public NoCoffeeMachinesException()
        {
        }

        public NoCoffeeMachinesException(string message) : base(message)
        {
        }

        public NoCoffeeMachinesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoCoffeeMachinesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}