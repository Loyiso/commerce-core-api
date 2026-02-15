namespace LoggingService.API.DTOs;

public sealed class LogResponse
{
    public Guid Id { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string? Service { get; set; }
    public string? CorrelationId { get; set; }
    public string? PropertiesJson { get; set; }
}
