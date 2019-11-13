using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;

namespace Cashier.Middleware
{
    /// <summary>
    /// Middleware to add X-Request-Id if not already set.
    /// </summary>
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for RequestIdMiddleware.
        /// </summary>
        /// <param name="next">Next Middleware.</param>
        public RequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Adds X-Request-Id if not already set.
        /// </summary>
        /// <param name="httpContext">HTTP Context.</param>
        /// <returns>Awaitable task.</returns>
        public Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            return InvokeInternalAsync(httpContext);
        }

        private async Task InvokeInternalAsync(HttpContext httpContext)
        {
            var requestIdFeature = httpContext.Features.Get<IHttpRequestIdentifierFeature>();
            if (requestIdFeature?.TraceIdentifier != null)
            {
                httpContext.Response.Headers["X-Request-Id"] = requestIdFeature.TraceIdentifier;
            }
            await _next(httpContext).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Middleware to add X-Request-Id if not already set.
    /// </summary>
    public static class RequestIdMiddlewareExtensions
    {
        /// <summary>
        /// Middleware to add X-Request-Id if not already set.
        /// </summary>
        /// <param name="builder">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseRequestIdMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestIdMiddleware>();
        }
    }
}
