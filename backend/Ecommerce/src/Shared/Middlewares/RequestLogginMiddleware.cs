using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Middlewares;

/// <summary>
/// Middleware responsible for intercepting HTTP requests, measuring execution latency, 
/// and logging formatted, color-coded status and performance metrics to the system console.
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    // ANSI Escape Color Codes for Terminal Console Styling
    private const string AnsiReset = "\u001B[0m";
    private const string AnsiRed = "\u001B[31m";
    private const string AnsiGreen = "\u001B[32m";
    private const string AnsiYellow = "\u001B[33m";
    private const string AnsiBlue = "\u001B[34m";
    private const string AnsiPurple = "\u001B[35m";
    private const string AnsiCyan = "\u001B[36m";
    private const string AnsiWhite = "\u001B[37m";

    private const string AnsiBoldRed = "\u001B[1;31m";
    private const string AnsiBoldGreen = "\u001B[1;32m";
    private const string AnsiBoldYellow = "\u001B[1;33m";
    private const string AnsiBoldCyan = "\u001B[1;36m";

    private static readonly Dictionary<string, string> MethodColors = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GET", AnsiGreen },
        { "POST", AnsiPurple },
        { "PUT", AnsiYellow },
        { "PATCH", AnsiYellow },
        { "DELETE", AnsiRed },
        { "OPTIONS", AnsiBlue },
        { "HEAD", AnsiWhite }
    };

    /// <summary>
    /// Processes the incoming HTTP context, captures total execution time, and outputs formatted log messages.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/> for the active HTTP request.</param>
    /// <returns>A task that represents the execution of the middleware pipeline.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Skip logging telemetry for API documentation routes and browser static assets
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // 2. Start high-precision timer and extract core HTTP request metadata
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        
        var method = request.Method;
        var fullUrl = $"{request.Path}{request.QueryString}";
        var clientIp = GetClientIp(context);

        try
        {
            // 3. Delegate execution to the next middleware component in the HTTP pipeline
            await _next(context);
        }
        finally
        {
            // 4. Calculate total elapsed duration and trigger colorized log formatting
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;
            var status = context.Response.StatusCode;

            LogFormattedRequest(method, status, fullUrl, clientIp, duration);
        }
    }

    /// <summary>
    /// Formats request parameters into ANSI colorized strings and dispatches the log payload to <see cref="ILogger"/>.
    /// </summary>
    /// <param name="method">The HTTP verb (e.g., GET, POST).</param>
    /// <param name="status">The returned HTTP status code.</param>
    /// <param name="fullUrl">The full request route path including query strings.</param>
    /// <param name="clientIp">The client remote IP address or proxy header.</param>
    /// <param name="duration">The execution duration in milliseconds.</param>
    private void LogFormattedRequest(string method, int status, string fullUrl, string clientIp, long duration)
    {
        // 1. Resolve HTTP method ANSI color style
        var methodColor = MethodColors.GetValueOrDefault(method, AnsiReset);

        // 2. Resolve HTTP status category color and label text based on status code ranges
        var (statusColor, statusLabel) = status switch
        {
            >= 500 => (AnsiBoldRed, "ERROR"),
            >= 400 => (AnsiBoldYellow, "CLIENT ERROR"),
            >= 300 => (AnsiBoldCyan, "REDIRECT"),
            >= 200 => (AnsiBoldGreen, "SUCCESS"),
            _ => (AnsiReset, "INFO")
        };

        // 3. Construct individual padded and color-formatted log segments
        var paddedMethod = $"{method,-7}";
        var coloredMethod = Colorize(paddedMethod, methodColor);
        var coloredStatus = Colorize(status.ToString(), statusColor);
        var coloredUrl = Colorize(fullUrl, AnsiWhite);
        var coloredIp = Colorize($"[{clientIp}]", AnsiBlue);

        // 4. Highlight execution latency based on performance threshold boundaries
        var durationColor = duration > 1000 ? AnsiYellow : (duration > 500 ? AnsiCyan : AnsiGreen);
        var coloredDuration = Colorize($"{duration}ms", durationColor);

        var coloredLabel = status >= 400 ? $" {Colorize($"[{statusLabel}]", statusColor)}" : string.Empty;

        // 5. Combine colorized tokens into a single formatted output string
        var formattedMessage = $"{coloredMethod} {coloredStatus} {coloredUrl} - {coloredIp} {coloredDuration}{coloredLabel}";

        // 6. Write to the underlying logging system using severity matching the HTTP status code
        if (status >= 500)
        {
            _logger.LogError("{LogMessage}", formattedMessage);
        }
        else if (status >= 400)
        {
            _logger.LogWarning("{LogMessage}", formattedMessage);
        }
        else
        {
            _logger.LogInformation("{LogMessage}", formattedMessage);
        }
    }

    /// <summary>
    /// Wraps text inside target ANSI color codes and attaches an ANSI reset sequence.
    /// </summary>
    /// <param name="text">The plain text to colorize.</param>
    /// <param name="color">The ANSI escape code sequence.</param>
    /// <returns>A formatted string ready for terminal rendering.</returns>
    private static string Colorize(string text, string color) => $"{color}{text}{AnsiReset}";

    /// <summary>
    /// Evaluates whether the incoming URL path matches static asset or telemetry exclusion filters.
    /// </summary>
    /// <param name="path">The relative path string from the current request.</param>
    /// <returns><c>true</c> if the path should bypass logging; otherwise, <c>false</c>.</returns>
    private static bool ShouldSkipLogging(PathString path)
    {
        // 1. Convert path to safe string and test against common non-business asset endpoints
        var pathValue = path.Value ?? string.Empty;
        return pathValue.Contains("/swagger", StringComparison.OrdinalIgnoreCase) ||
               pathValue.Contains("/favicon.ico", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Inspects HTTP headers to resolve the true client IP address behind proxies, load balancers, or CDN networks.
    /// </summary>
    /// <param name="context">The active HTTP request context.</param>
    /// <returns>The resolved client IP address string.</returns>
    private static string GetClientIp(HttpContext context)
    {
        // 1. Define standard forward proxy headers in order of priority
        var headers = new[] { "X-Forwarded-For", "X-Real-IP", "Proxy-Client-IP", "WL-Proxy-Client-IP" };

        // 2. Search incoming request headers for client remote address
        foreach (var header in headers)
        {
            if (context.Request.Headers.TryGetValue(header, out var values))
            {
                var ip = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(ip) && !ip.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                {
                    return ip.Split(',')[0].Trim();
                }
            }
        }

        // 3. Fall back to local remote connection address if no proxy headers are present
        return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}