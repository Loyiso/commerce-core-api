using System.ComponentModel.DataAnnotations;

namespace Shared.Logging.Entities;

public sealed class LogEntryEntity
{
    [Key]
    public long Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    [MaxLength(32)]
    public string Level { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    [MaxLength(128)]
    public string Service { get; set; } = string.Empty;

    [MaxLength(64)]
    public string Environment { get; set; } = string.Empty;

    public string PropertiesJson { get; set; } = "{}";
}
