using CatalogService.API.Application.Common;
using CatalogService.API.Application.DTOs;
using CatalogService.API.Application.Interfaces;

namespace CatalogService.API.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<PagedResult<ProductDto>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var result = await _repo.GetAsync(search, sortBy, desc, page, pageSize, ct);

        return new PagedResult<ProductDto>
        {
            Items = result.Items.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        if (p is null) return null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl
        };
    }
}
