
using CatalogService.API.Application.DTOs;

namespace CatalogService.API.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize);

    Task<ProductDto?> GetByIdAsync(Guid id);
}
