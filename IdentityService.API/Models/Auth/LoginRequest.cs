namespace IdentityService.API.Models.Auth;

public sealed record LoginRequest(
    string EmailOrUsername,
    string Password
);
