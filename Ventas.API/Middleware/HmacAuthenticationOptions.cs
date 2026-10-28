namespace Ventas.API.Middleware;

public sealed class HmacAuthenticationOptions
{
    public const string SectionName = "HmacAuthentication";

    public int AllowedClockSkewSeconds { get; set; } = 300;

    public Dictionary<string, string> Clients { get; set; } = new(StringComparer.Ordinal);
}
