namespace LoggingService.API.Models;

public sealed class LogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>UTC timestamp of the log.</summary>
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Log level (Information, Warning, Error, etc.)</summary>
    public string Level { get; set; } = "Information";

    /// <summary>Message template or rendered message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional service name emitting the log (e.g., CatalogService.API).</summary>
    public string? Service { get; set; }

    /// <summary>Optional correlation id / trace id.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Optional JSON payload for structured properties.</summary>
    public string? PropertiesJson { get; set; }
}
