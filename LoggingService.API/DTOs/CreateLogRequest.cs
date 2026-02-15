using System.ComponentModel.DataAnnotations;

namespace LoggingService.API.DTOs;

public sealed class CreateLogRequest
{
    /// <summary>Level like Information, Warning, Error.</summary>
    [Required, MaxLength(32)]
    public string Level { get; set; } = "Information";

    [Required, MaxLength(4000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? Service { get; set; }

    [MaxLength(128)]
    public string? CorrelationId { get; set; }

    /// <summary>JSON payload for structured properties (optional).</summary>
    public string? PropertiesJson { get; set; }

    /// <summary>Optional client-provided timestamp (UTC). If omitted, server uses now.</summary>
    public DateTimeOffset? TimestampUtc { get; set; }
}
