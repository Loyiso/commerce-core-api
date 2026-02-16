using IdentityService.API.Models;

namespace IdentityService.API.Services;

public interface ILoggingApiClient
{
    Task LogAsync(LoggingApiRequest request, CancellationToken ct = default);
}
