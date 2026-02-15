namespace CatalogService.API.Infrastructure;

public sealed class LoggingApiRequest
{
    public string Service { get; init; } = "CatalogService.API";
    public string Level { get; init; } = "Information"; // Information|Warning|Error|Debug
    public string Message { get; init; } = string.Empty;
    public string? Exception { get; init; }
    public string? TraceId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public Dictionary<string, object?> Properties { get; init; } = new();
}
