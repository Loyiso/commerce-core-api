
using CatalogService.API.Application.DTOs;
using CatalogService.API.Application.Interfaces;
using CatalogService.API.Domain.Entities;

namespace CatalogService.API.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductDto>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize)
    {
        var products = await _repo.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
            products = products.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        products = sortBy?.ToLower() switch
        {
            "price" => desc ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
            _ => desc ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name)
        };

        products = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl
        });
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;

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
