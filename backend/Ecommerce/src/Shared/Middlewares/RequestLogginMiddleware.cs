using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Middlewares;

// Middleware for intercepting HTTP requests and logging detailed performance and status metrics in color.
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    // ANSI Color Codes for Console Output
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

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for documentation and static assets
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        
        var method = request.Method;
        var fullUrl = $"{request.Path}{request.QueryString}";
        var clientIp = GetClientIp(context);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;
            var status = context.Response.StatusCode;

            LogFormattedRequest(method, status, fullUrl, clientIp, duration);
        }
    }

    private void LogFormattedRequest(string method, int status, string fullUrl, string clientIp, long duration)
    {
        var methodColor = MethodColors.GetValueOrDefault(method, AnsiReset);

        var (statusColor, statusLabel) = status switch
        {
            >= 500 => (AnsiBoldRed, "ERROR"),
            >= 400 => (AnsiBoldYellow, "CLIENT ERROR"),
            >= 300 => (AnsiBoldCyan, "REDIRECT"),
            >= 200 => (AnsiBoldGreen, "SUCCESS"),
            _ => (AnsiReset, "INFO")
        };

        var paddedMethod = $"{method,-7}";
        var coloredMethod = Colorize(paddedMethod, methodColor);
        var coloredStatus = Colorize(status.ToString(), statusColor);
        var coloredUrl = Colorize(fullUrl, AnsiWhite);
        var coloredIp = Colorize($"[{clientIp}]", AnsiBlue);

        var durationColor = duration > 1000 ? AnsiYellow : (duration > 500 ? AnsiCyan : AnsiGreen);
        var coloredDuration = Colorize($"{duration}ms", durationColor);

        var coloredLabel = status >= 400 ? $" {Colorize($"[{statusLabel}]", statusColor)}" : string.Empty;

        var formattedMessage = $"{coloredMethod} {coloredStatus} {coloredUrl} - {coloredIp} {coloredDuration}{coloredLabel}";

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

    private static string Colorize(string text, string color) => $"{color}{text}{AnsiReset}";

    private static bool ShouldSkipLogging(PathString path)
    {
        var pathValue = path.Value ?? string.Empty;
        return pathValue.Contains("/swagger", StringComparison.OrdinalIgnoreCase) ||
               pathValue.Contains("/favicon.ico", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetClientIp(HttpContext context)
    {
        var headers = new[] { "X-Forwarded-For", "X-Real-IP", "Proxy-Client-IP", "WL-Proxy-Client-IP" };

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

        return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}