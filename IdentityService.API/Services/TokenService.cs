using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.API.Services;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public (string token, DateTime expiresAtUtc) CreateAccessToken(AppUser user)
    {
        var jwt = _config.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var signingKey = jwt["SigningKey"]!;
        var minutes = int.Parse(jwt["AccessTokenMinutes"] ?? "15");

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public (string refreshToken, DateTime expiresAtUtc, string refreshTokenHash) CreateRefreshToken()
    {
        var jwt = _config.GetSection("Jwt");
        var days = int.Parse(jwt["RefreshTokenDays"] ?? "14");

        // 32 bytes -> base64url string
        var rawBytes = RandomNumberGenerator.GetBytes(64);
        var raw = Base64UrlEncoder.Encode(rawBytes);
        var hash = HashToken(raw);
        return (raw, DateTime.UtcNow.AddDays(days), hash);
    }

    public string HashToken(string token)
    {
        // SHA256 hash, hex
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
