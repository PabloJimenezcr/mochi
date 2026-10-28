using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ventas.API.Middleware;
using Xunit;

namespace Ventas.Tests.Middleware;

public class HmacAuthenticationMiddlewareTests
{
    private const string ApiKey = "test-client";
    private const string Secret = "test-secret";
    private const long CurrentTimestamp = 1_700_000_000;

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenRequestHasValidHmacHeaders()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        AddAuthenticationHeaders(context);
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenHeadersAreMissing()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenSignatureIsInvalid()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        context.Request.Headers["X-Api-Key"] = ApiKey;
        context.Request.Headers["X-Timestamp"] = CurrentTimestamp.ToString();
        context.Request.Headers["X-Signature"] = "invalid-signature";
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenApiKeyIsUnknown()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        context.Request.Headers["X-Api-Key"] = "unknown-client";
        context.Request.Headers["X-Timestamp"] = CurrentTimestamp.ToString();
        context.Request.Headers["X-Signature"] = HmacAuthenticationMiddleware.CreateSignature(
            context.Request.Method,
            context.Request.Path.Value!,
            CurrentTimestamp.ToString(),
            Secret);
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenTimestampIsOutsideAllowedWindow()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        var oldTimestamp = CurrentTimestamp - 301;
        context.Request.Headers["X-Api-Key"] = ApiKey;
        context.Request.Headers["X-Timestamp"] = oldTimestamp.ToString();
        context.Request.Headers["X-Signature"] = HmacAuthenticationMiddleware.CreateSignature(
            context.Request.Method,
            context.Request.Path.Value!,
            oldTimestamp.ToString(),
            Secret);
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenTimestampIsNotNumeric()
    {
        var nextCalled = false;
        var context = CreateContext("GET", "/api/categorias");
        context.Request.Headers["X-Api-Key"] = ApiKey;
        context.Request.Headers["X-Timestamp"] = "not-a-timestamp";
        context.Request.Headers["X-Signature"] = "invalid";
        var middleware = CreateMiddleware(() => nextCalled = true);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    private static DefaultHttpContext CreateContext(string method, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        return context;
    }

    private static void AddAuthenticationHeaders(DefaultHttpContext context)
    {
        var timestamp = CurrentTimestamp.ToString();
        context.Request.Headers["X-Api-Key"] = ApiKey;
        context.Request.Headers["X-Timestamp"] = timestamp;
        context.Request.Headers["X-Signature"] = HmacAuthenticationMiddleware.CreateSignature(
            context.Request.Method,
            context.Request.Path.Value!,
            timestamp,
            Secret);
    }

    private static HmacAuthenticationMiddleware CreateMiddleware(Action nextAction)
    {
        RequestDelegate next = _ =>
        {
            nextAction();
            return Task.CompletedTask;
        };

        var options = Options.Create(new HmacAuthenticationOptions
        {
            AllowedClockSkewSeconds = 300,
            Clients = new Dictionary<string, string>
            {
                [ApiKey] = Secret
            }
        });

        return new HmacAuthenticationMiddleware(
            next,
            options,
            new FixedTimeProvider(DateTimeOffset.FromUnixTimeSeconds(CurrentTimestamp)));
    }

    private sealed class FixedTimeProvider(DateTimeOffset currentTime) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => currentTime;
    }
}
