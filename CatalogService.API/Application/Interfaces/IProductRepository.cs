
using CatalogService.API.Domain.Entities; 

namespace CatalogService.API.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
}
