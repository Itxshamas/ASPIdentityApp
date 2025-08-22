using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ASPIdentityApp.Middleware
{
    public class JwtLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtLoggingMiddleware> _logger;

        public JwtLoggingMiddleware(RequestDelegate next, ILogger<JwtLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                // log partial token for privacy
                var token = authHeader.Split(' ').LastOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Authorization header present. Token starts with: {start}", token.Substring(0, Math.Min(10, token.Length)));
                }
            }
            await _next(context);
        }
    }
}
