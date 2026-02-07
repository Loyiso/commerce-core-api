using IdentityService.API.Models.Auth;

namespace IdentityService.API.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, string? userAgent, string? ipAddress, CancellationToken ct = default);
    Task LogoutAsync(RefreshRequest request, CancellationToken ct = default);
}
