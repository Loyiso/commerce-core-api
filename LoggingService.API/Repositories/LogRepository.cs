using LoggingService.API.Data;
using LoggingService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoggingService.API.Repositories;

public sealed class LogRepository : ILogRepository
{
    private readonly LoggingDbContext _db;

    public LogRepository(LoggingDbContext db) => _db = db;

    public async Task<LogEntry> AddAsync(LogEntry entry, CancellationToken ct)
    {
        _db.Logs.Add(entry);
        await _db.SaveChangesAsync(ct);
        return entry;
    }

    public Task<LogEntry?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Logs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<LogEntry> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 50 : Math.Min(pageSize, 500);

        var query = _db.Logs.AsNoTracking().OrderByDescending(x => x.TimestampUtc);
        var total = await query.LongCountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
