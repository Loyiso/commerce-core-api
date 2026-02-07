using IdentityService.API.Entities;

namespace IdentityService.API.Services;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(AppUser user); 
    (string refreshToken, DateTime expiresAtUtc, string refreshTokenHash) CreateRefreshToken();
    string HashToken(string token);
}
