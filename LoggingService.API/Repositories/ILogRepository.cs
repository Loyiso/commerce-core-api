using LoggingService.API.Models;

namespace LoggingService.API.Repositories;

public interface ILogRepository
{
    Task<LogEntry> AddAsync(LogEntry entry, CancellationToken ct);
    Task<LogEntry?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<LogEntry> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct);
}
