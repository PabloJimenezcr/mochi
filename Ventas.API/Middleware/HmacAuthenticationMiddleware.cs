using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Ventas.API.Middleware;

public sealed class HmacAuthenticationMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";
    private const string TimestampHeader = "X-Timestamp";
    private const string SignatureHeader = "X-Signature";

    private readonly RequestDelegate _next;
    private readonly HmacAuthenticationOptions _options;
    private readonly TimeProvider _timeProvider;

    public HmacAuthenticationMiddleware(
        RequestDelegate next,
        IOptions<HmacAuthenticationOptions> options,
        TimeProvider? timeProvider = null)
    {
        _next = next;
        _options = options.Value;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!TryGetHeader(context, ApiKeyHeader, out var apiKey) ||
            !TryGetHeader(context, TimestampHeader, out var timestampValue) ||
            !TryGetHeader(context, SignatureHeader, out var signatureValue) ||
            !_options.Clients.TryGetValue(apiKey, out var secret) ||
            !long.TryParse(timestampValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestamp) ||
            !IsTimestampValid(timestamp) ||
            !IsSignatureValid(context, timestampValue, signatureValue, secret))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }

    public static string CreateSignature(string method, string path, string timestamp, string secret)
    {
        var canonicalString = BuildCanonicalString(method, path, timestamp);
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(canonicalString);
        var hash = HMACSHA256.HashData(key, data);

        return Convert.ToBase64String(hash);
    }

    private bool IsSignatureValid(
        HttpContext context,
        string timestamp,
        string providedSignature,
        string secret)
    {
        var expectedSignature = CreateSignature(
            context.Request.Method,
            context.Request.Path.Value ?? "/",
            timestamp,
            secret);

        try
        {
            var expectedBytes = Convert.FromBase64String(expectedSignature);
            var providedBytes = Convert.FromBase64String(providedSignature);

            return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private bool IsTimestampValid(long timestamp)
    {
        var now = _timeProvider.GetUtcNow().ToUnixTimeSeconds();
        var difference = timestamp - now;

        return Math.Abs(difference) <= Math.Max(0, _options.AllowedClockSkewSeconds);
    }

    private static string BuildCanonicalString(string method, string path, string timestamp)
    {
        return $"{method.ToUpperInvariant()}\n{path}\n{timestamp}";
    }

    private static bool TryGetHeader(HttpContext context, string headerName, out string value)
    {
        if (context.Request.Headers.TryGetValue(headerName, out var headerValues) &&
            headerValues.Count == 1 &&
            !string.IsNullOrWhiteSpace(headerValues[0]))
        {
            value = headerValues[0]!;
            return true;
        }

        value = string.Empty;
        return false;
    }
}
