using IdentityService.API.Data;
using IdentityService.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => _db.RefreshTokens.AddAsync(token, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
