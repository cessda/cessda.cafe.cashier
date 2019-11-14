using System;
using System.Threading.Tasks;

namespace Cashier.Engine
{
    /// <summary>
    /// Exposes the order engine, which sends coffees to remote coffee machines.
    /// </summary>
    public interface IOrderEngine
    {
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
