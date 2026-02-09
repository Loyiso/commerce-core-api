using CartService.API.Data;
using CartService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly CartDbContext _db;

    public CartRepository(CartDbContext db) => _db = db;

    public Task<List<Cart>> GetAllAsync(CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items)
            .OrderByDescending(c => c.UpdatedAtUtc)
            .ToListAsync(ct);

    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Cart>> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAtUtc)
            .ToListAsync(ct);

    public async Task<Cart> AddAsync(Cart cart, CancellationToken ct = default)
    {
        cart.CreatedAtUtc = DateTime.UtcNow;
        cart.UpdatedAtUtc = DateTime.UtcNow;

        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(ct);
        return cart;
    }

    public async Task<Cart?> UpdateAsync(Cart cart, CancellationToken ct = default)
    {
        var existing = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cart.Id, ct);
        if (existing is null) return null;

        existing.UserId = cart.UserId;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        // Full replace items for simplicity (cascade delete ensures cleanup)
        existing.Items.Clear();
        foreach (var item in cart.Items)
        {
            existing.Items.Add(new CartItem
            {
                CartId = existing.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                AddedAtUtc = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Carts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (existing is null) return false;

        _db.Carts.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
