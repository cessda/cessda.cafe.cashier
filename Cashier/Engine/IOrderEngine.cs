using System;

namespace Cashier.Engine
{
    /// <summary>
    /// Exposes the order engine, which sends coffees to remote coffee machines.
    /// </summary>
    public interface IOrderEngine
    {
        /// <summary>
        /// Starts the specified order.
        /// </summary>
        /// <param name="id">The OrderId to start.</param>
        void StartOrder(Guid id);

        /// <summary>
        /// Starts the specified coffee.
        /// </summary>
        /// <param name="id">The jobId to start.</param>
        void StartCoffee(Guid id);
    }
}
