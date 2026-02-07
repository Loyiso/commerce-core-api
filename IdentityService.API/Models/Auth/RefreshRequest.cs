namespace IdentityService.API.Models.Auth;

public sealed record RefreshRequest(
    string RefreshToken
);
