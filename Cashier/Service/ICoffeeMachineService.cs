using System;
using System.Threading.Tasks;

namespace Cessda.Cafe.Cashier.Service
{
    /// <summary>
    /// Exposes the order engine, which sends coffees to remote coffee machines.
    /// </summary>
    public interface ICoffeeMachineService
    {
        /// <summary>
        /// Starts the jobs associated with an order.
        /// </summary>
        /// <param name="id">The order to start.</param>
        Task StartOrderAsync(Guid id);

        /// <summary>
        /// Starts all jobs that haven't been started.
        /// </summary>
        Task StartAllJobsAsync();

        /// <summary>
        /// Starts the specified coffee.
        /// </summary>
        /// <param name="id">The jobId to start.</param>
        Task StartJobAsync(Guid id);
    }
}
