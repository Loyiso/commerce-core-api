using CartService.API.Contracts;

namespace CartService.API.Services;

public interface ICartService
{
    Task<List<CartResponse>> GetAllAsync(CancellationToken ct = default);
    Task<CartResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<CartResponse>> GetByUserIdAsync(string userId, CancellationToken ct = default);

    Task<CartResponse> CreateAsync(CartCreateRequest request, CancellationToken ct = default);
    Task<CartResponse?> UpdateAsync(Guid id, CartUpdateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
