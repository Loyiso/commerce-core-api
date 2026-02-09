using Microsoft.EntityFrameworkCore;
using UserService.API.Data;
using UserService.API.Models;

namespace UserService.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _db;

    public UserRepository(UsersDbContext db) => _db = db;

    public Task<List<UserProfile>> GetAllAsync(CancellationToken ct = default) =>
        _db.Users
           .OrderByDescending(u => u.UpdatedAtUtc)
           .ToListAsync(ct);

    public Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<UserProfile> AddAsync(UserProfile user, CancellationToken ct = default)
    {
        user.CreatedAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<UserProfile?> UpdateAsync(UserProfile user, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id, ct);
        if (existing is null) return null;

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.Phone = user.Phone;

        existing.Address.Line1 = user.Address.Line1;
        existing.Address.Line2 = user.Address.Line2;
        existing.Address.City = user.Address.City;
        existing.Address.Province = user.Address.Province;
        existing.Address.PostalCode = user.Address.PostalCode;
        existing.Address.Country = user.Address.Country;

        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is null) return false;

        _db.Users.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
