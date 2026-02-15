using LoggingService.API.DTOs;
using LoggingService.API.Models;
using LoggingService.API.Repositories;

namespace LoggingService.API.Services;

public sealed class LogService : ILogService
{
    private readonly ILogRepository _repo;

    public LogService(ILogRepository repo) => _repo = repo;

    public async Task<LogResponse> CreateAsync(CreateLogRequest request, CancellationToken ct)
    {
        var entry = new LogEntry
        {
            Id = Guid.NewGuid(),
            TimestampUtc = request.TimestampUtc ?? DateTimeOffset.UtcNow,
            Level = request.Level,
            Message = request.Message,
            Service = request.Service,
            CorrelationId = request.CorrelationId,
            PropertiesJson = request.PropertiesJson
        };

        await _repo.AddAsync(entry, ct);
        return Map(entry);
    }

    public async Task<LogResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entry = await _repo.GetByIdAsync(id, ct);
        return entry is null ? null : Map(entry);
    }

    public async Task<PagedResult<LogResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(pageNumber, pageSize, ct);

        return new PagedResult<LogResponse>
        {
            Items = items.Select(Map).ToList(),
            PageNumber = pageNumber <= 0 ? 1 : pageNumber,
            PageSize = pageSize <= 0 ? 50 : Math.Min(pageSize, 500),
            TotalCount = total
        };
    }

    private static LogResponse Map(LogEntry x) => new()
    {
        Id = x.Id,
        TimestampUtc = x.TimestampUtc,
        Level = x.Level,
        Message = x.Message,
        Service = x.Service,
        CorrelationId = x.CorrelationId,
        PropertiesJson = x.PropertiesJson
    };
}
