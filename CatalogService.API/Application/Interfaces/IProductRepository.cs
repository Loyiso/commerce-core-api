using CatalogService.API.Application.Common;
using CatalogService.API.Domain.Entities;

namespace CatalogService.API.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
