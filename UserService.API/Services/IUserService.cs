using UserService.API.Contracts;

namespace UserService.API.Services;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync(CancellationToken ct = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse> CreateAsync(UserCreateRequest request, CancellationToken ct = default);
    Task<UserResponse?> UpdateAsync(Guid id, UserUpdateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
