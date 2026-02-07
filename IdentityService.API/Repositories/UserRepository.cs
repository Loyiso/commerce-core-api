using IdentityService.API.Data;
using IdentityService.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), ct);

    public Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower(), ct);

    public Task<AppUser?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken ct = default)
    {
        var key = emailOrUsername.ToLower();
        return _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == key || x.Username.ToLower() == key, ct);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => _db.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower(), ct);

    public Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
        => _db.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower(), ct);

    public Task AddAsync(AppUser user, CancellationToken ct = default)
        => _db.Users.AddAsync(user, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
