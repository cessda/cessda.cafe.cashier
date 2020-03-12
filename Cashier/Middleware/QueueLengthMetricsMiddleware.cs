using Cashier.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cashier.Middleware
{
    /// <summary>
    /// Middleware to expose Prometheus metrics for the queue length of the cashier
    /// </summary>
    public class QueueLengthMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Gauge gauge = Metrics.CreateGauge("cashier_queue_length", "The current queue length.");

        /// <summary>
        /// DI Constructor
        /// </summary>
        public QueueLengthMetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Set the queue length for the Prometheus endpoint
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext, CashierDbContext context)
        {
            ValidateParameters(context);

            gauge.Set(await context.Jobs.Where(c => string.IsNullOrEmpty(c.Machine)).CountAsync());

            await _next(httpContext);
        }

        private static void ValidateParameters(CashierDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class QueueLengthMetricsMiddlewareExtensions
    {
        /// <summary>
        /// Extension method used to add the middleware to the HTTP request pipeline.
        /// </summary>
        public static IApplicationBuilder UseQueueLengthMetricsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<QueueLengthMetricsMiddleware>();
        }
    }
}
