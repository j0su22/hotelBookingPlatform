using System.Text;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;

namespace HotelBookingPlatform.Api.Middleware;

public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    private const string IdempotencyKeyHeader = "Idempotency-Key";

    public async Task InvokeAsync(HttpContext context, IIdempotencyRepository idempotencyRepo)
    {
        if (!IsIdempotentMethod(context.Request.Method))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var keyValues))
        {
            await next(context);
            return;
        }

        var key = keyValues.ToString();
        if (string.IsNullOrWhiteSpace(key))
        {
            await next(context);
            return;
        }

        var existing = await idempotencyRepo.GetByKeyAsync(key, context.RequestAborted);
        if (existing is not null && !existing.IsExpired())
        {
            context.Response.StatusCode = existing.ResponseStatus;
            context.Response.ContentType = "application/json";
            context.Response.Headers["X-Idempotency-Replayed"] = "true";
            await context.Response.WriteAsync(existing.ResponseBody, context.RequestAborted);
            return;
        }

        var originalBody = context.Response.Body;
        await using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await next(context);

        memStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memStream).ReadToEndAsync(context.RequestAborted);

        var record = IdempotencyRecord.Create(key, context.Response.StatusCode, responseBody);
        try { await idempotencyRepo.SaveAsync(record, context.RequestAborted); } catch { /* ignore duplicate key */ }

        memStream.Seek(0, SeekOrigin.Begin);
        context.Response.Body = originalBody;
        await context.Response.WriteAsync(responseBody, context.RequestAborted);
    }

    private static bool IsIdempotentMethod(string method) =>
        method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
        method.Equals("PUT", StringComparison.OrdinalIgnoreCase);
}
