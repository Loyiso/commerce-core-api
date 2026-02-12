using Microsoft.EntityFrameworkCore;
using Shared.Logging.Data;
using Shared.Logging.Entities;

namespace Shared.Logging.Store;

public sealed class InMemoryLogStore : ILogStore
{
    private readonly IDbContextFactory<LoggingDbContext> _dbFactory;

    public InMemoryLogStore(IDbContextFactory<LoggingDbContext> dbFactory)
        => _dbFactory = dbFactory;

    public async Task AddAsync(LogEntryEntity entry, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        db.Logs.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LogEntryEntity>> GetLatestAsync(int take = 200, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Logs
            .OrderByDescending(x => x.Timestamp)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        db.Logs.RemoveRange(db.Logs);
        await db.SaveChangesAsync(ct);
    }
}
