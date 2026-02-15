namespace LoggingService.API.DTOs;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required long TotalCount { get; init; }

    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
}
