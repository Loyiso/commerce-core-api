
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
    {
        try
        {
            return await _context.Products.ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
            Console.WriteLine($"An error occurred while fetching products: {ex.Message}");
            return Enumerable.Empty<Product>(); // Return an empty list in case of an error
        }
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Products.FindAsync(id);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"An error occurred while fetching the product with ID {id}: {ex.Message}");
            return null; // Return null in case of an error
        }
    }
}
