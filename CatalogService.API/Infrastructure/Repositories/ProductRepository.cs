using CatalogService.API.Application.Common;
using CatalogService.API.Application.Interfaces;
using CatalogService.API.Domain.Entities;
using CatalogService.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<Product>> GetAsync(
        string? search,
        string? sortBy,
        bool desc,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 200 ? 200 : pageSize;

        try
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(p => p.Name.Contains(s));
            }

            query = (sortBy?.ToLower()) switch
            {
                "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                _ => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
            };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PagedResult<Product>
            {
                Items = Array.Empty<Product>(),
                TotalCount = 0,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
}
