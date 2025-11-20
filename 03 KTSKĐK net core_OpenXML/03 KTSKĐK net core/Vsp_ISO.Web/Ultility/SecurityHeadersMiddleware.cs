using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace VSP_HealthExam.Web.Ultility
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Add security headers
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

                // Development vs Production CSP
                string cspHeader;
                if (_env.IsDevelopment())
                {
                    // More permissive CSP for development (allows browser refresh, fonts, etc.)
                    cspHeader = "default-src 'self' localhost:* 127.0.0.1:*; " +
                               "script-src 'self' 'unsafe-inline' 'unsafe-eval' localhost:* 127.0.0.1:*; " +
                               "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com localhost:* 127.0.0.1:*; " +
                               "style-src-elem 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                               "img-src 'self' data: https: localhost:* 127.0.0.1:*; " +
                               "font-src 'self' data: https://fonts.gstatic.com localhost:* 127.0.0.1:*; " +
                               "connect-src 'self' ws://localhost:* wss://localhost:* ws://127.0.0.1:* wss://127.0.0.1:* http://localhost:* https://localhost:*; " +
                               "frame-ancestors 'none'; " +
                               "base-uri 'self';";
                }
                else
                {
                    // Strict CSP for production
                    cspHeader = "default-src 'self'; " +
                               "script-src 'self' 'unsafe-inline'; " +
                               "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                               "style-src-elem 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                               "img-src 'self' data: https:; " +
                               "font-src 'self' data: https://fonts.gstatic.com; " +
                               "connect-src 'self'; " +
                               "frame-ancestors 'none'; " +
                               "base-uri 'self';";
                }

                context.Response.Headers.Add("Content-Security-Policy", cspHeader);

                // Add cache control headers for sensitive pages
                if (context.Request.Path.StartsWithSegments("/NgoaiVSP") ||
                    context.Request.Path.StartsWithSegments("/Account"))
                {
                    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                    context.Response.Headers.Add("Pragma", "no-cache");
                    context.Response.Headers.Add("Expires", "0");
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SecurityHeadersMiddleware");
                throw;
            }
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}