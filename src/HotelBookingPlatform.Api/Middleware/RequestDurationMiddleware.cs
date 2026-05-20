using System.Diagnostics;

namespace HotelBookingPlatform.Api.Middleware;

public sealed class RequestDurationMiddleware(RequestDelegate next, ILogger<RequestDurationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        logger.LogInformation(
            "[HTTP {Method} {Path}] completed in {ElapsedMs}ms with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            sw.ElapsedMilliseconds,
            context.Response.StatusCode);
    }
}
