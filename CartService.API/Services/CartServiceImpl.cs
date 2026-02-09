using CartService.API.Contracts;
using CartService.API.Models;
using CartService.API.Repositories;

namespace CartService.API.Services;

public class CartServiceImpl : ICartService
{
    private readonly ICartRepository _repo;

    public CartServiceImpl(ICartRepository repo) => _repo = repo;

    public async Task<List<CartResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var carts = await _repo.GetAllAsync(ct);
        return carts.Select(ToResponse).ToList();
    }

    public async Task<CartResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cart = await _repo.GetByIdAsync(id, ct);
        return cart is null ? null : ToResponse(cart);
    }

    public async Task<List<CartResponse>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var carts = await _repo.GetByUserIdAsync(userId, ct);
        return carts.Select(ToResponse).ToList();
    }

    public async Task<CartResponse> CreateAsync(CartCreateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("UserId is required.");

        var cart = new Cart
        {
            UserId = request.UserId,
            Items = request.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                Quantity = Math.Max(1, i.Quantity),
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        // Ensure FK set (EF InMemory won't auto set if entity not tracked yet)
        foreach (var item in cart.Items)
            item.CartId = cart.Id;

        var created = await _repo.AddAsync(cart, ct);
        return ToResponse(created);
    }

    public async Task<CartResponse?> UpdateAsync(Guid id, CartUpdateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("UserId is required.");

        var cart = new Cart
        {
            Id = id,
            UserId = request.UserId,
            Items = request.Items.Select(i => new CartItem
            {
                CartId = id,
                ProductId = i.ProductId,
                Quantity = Math.Max(1, i.Quantity),
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var updated = await _repo.UpdateAsync(cart, ct);
        return updated is null ? null : ToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) =>
        _repo.DeleteAsync(id, ct);

    private static CartResponse ToResponse(Cart cart) =>
        new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAtUtc = cart.CreatedAtUtc,
            UpdatedAtUtc = cart.UpdatedAtUtc,
            Items = cart.Items.Select(i => new CartItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                AddedAtUtc = i.AddedAtUtc
            }).ToList()
        };
}
