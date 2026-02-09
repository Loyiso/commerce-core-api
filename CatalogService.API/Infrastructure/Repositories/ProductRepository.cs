
using CatalogService.API.Application.Interfaces;
using CatalogService.API.Domain.Entities; 
using CatalogService.API.Infrastructure.Data;  
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
}
