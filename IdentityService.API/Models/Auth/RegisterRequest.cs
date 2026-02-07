namespace IdentityService.API.Models.Auth;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password
);
