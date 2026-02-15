using LoggingService.API.DTOs;

namespace LoggingService.API.Services;

public interface ILogService
{
    Task<LogResponse> CreateAsync(CreateLogRequest request, CancellationToken ct);
    Task<LogResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<LogResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct);
}
