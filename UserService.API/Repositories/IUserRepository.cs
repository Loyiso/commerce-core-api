using UserService.API.Models;

namespace UserService.API.Repositories;

public interface IUserRepository
{
    Task<List<UserProfile>> GetAllAsync(CancellationToken ct = default);
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserProfile> AddAsync(UserProfile user, CancellationToken ct = default);
    Task<UserProfile?> UpdateAsync(UserProfile user, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
