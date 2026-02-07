namespace IdentityService.API.Models.Auth;

public sealed record MeResponse(
    Guid UserId,
    string Email,
    string Username,
    string Role
);
