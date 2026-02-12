using Shared.Logging.Entities;

namespace Shared.Logging.Store;

public interface ILogStore
{
    Task AddAsync(LogEntryEntity entry, CancellationToken ct = default);
    Task<IReadOnlyList<LogEntryEntity>> GetLatestAsync(int take = 200, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
}
