using CartService.API.Models;

namespace CartService.API.Repositories;

public interface ICartRepository
{
    Task<List<Cart>> GetAllAsync(CancellationToken ct = default);
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Cart>> GetByUserIdAsync(string userId, CancellationToken ct = default);

    Task<Cart> AddAsync(Cart cart, CancellationToken ct = default);
    Task<Cart?> UpdateAsync(Cart cart, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
