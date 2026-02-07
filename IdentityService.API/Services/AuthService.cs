using IdentityService.API.Entities;
using IdentityService.API.Models.Auth;
using IdentityService.API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ITokenService _tokens;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ITokenService tokens)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _tokens = tokens;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default)
    {
        if (await _users.EmailExistsAsync(request.Email, ct))
            throw new InvalidOperationException("Email is already registered.");

        if (await _users.UsernameExistsAsync(request.Username, ct))
            throw new InvalidOperationException("Username is already taken.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            Username = request.Username.Trim(),
            Role = "User",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, userAgent, ipAddress, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailOrUsernameAsync(request.EmailOrUsername.Trim(), ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await IssueTokensAsync(user, userAgent, ipAddress, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default)
    {
        var presentedHash = _tokens.HashToken(request.RefreshToken);

        var stored = await _refreshTokens.GetByTokenHashAsync(presentedHash, ct);
        if (stored is null || stored.User is null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!stored.IsActive)
            throw new UnauthorizedAccessException("Refresh token is not active.");

        // Rotation: revoke old, issue new
        stored.RevokedAtUtc = DateTime.UtcNow;

        var (newRaw, newExp, newHash) = _tokens.CreateRefreshToken();
        stored.ReplacedByTokenHash = newHash;

        var newTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.UserId,
            TokenHash = newHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = newExp,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };

        await _refreshTokens.AddAsync(newTokenEntity, ct);
        await _refreshTokens.SaveChangesAsync(ct);

        var (access, accessExp) = _tokens.CreateAccessToken(stored.User);

        return new AuthResponse(access, accessExp, newRaw, newExp);
    }

    public async Task LogoutAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var presentedHash = _tokens.HashToken(request.RefreshToken);
        var stored = await _refreshTokens.GetByTokenHashAsync(presentedHash, ct);

        if (stored is null)
            return;

        stored.RevokedAtUtc = DateTime.UtcNow;
        await _refreshTokens.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> IssueTokensAsync(AppUser user, string? userAgent, string? ipAddress, CancellationToken ct)
    {
        var (access, accessExp) = _tokens.CreateAccessToken(user);
        var (refreshRaw, refreshExp, refreshHash) = _tokens.CreateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = refreshExp,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };

        await _refreshTokens.AddAsync(refreshEntity, ct);
        await _refreshTokens.SaveChangesAsync(ct);

        return new AuthResponse(access, accessExp, refreshRaw, refreshExp);
    }
}
