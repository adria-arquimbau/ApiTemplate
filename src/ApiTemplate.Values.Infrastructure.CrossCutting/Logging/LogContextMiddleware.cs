using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Values.Infrastructure.CrossCutting.Logging
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogContextMiddleware> logger;

        public LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationHeaders = Activity.Current.Baggage.Distinct().ToDictionary(b => b.Key, b => (object) b.Value);

            // ensures all entries are tagged with some values
            using (logger.BeginScope(correlationHeaders))
            {
                // Call the next delegate/middleware in the pipeline
                await next(context);
            }
        }
    }
}
