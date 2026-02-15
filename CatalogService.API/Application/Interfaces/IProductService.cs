using CatalogService.API.Application.Common;
using CatalogService.API.Application.DTOs;

namespace CatalogService.API.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
