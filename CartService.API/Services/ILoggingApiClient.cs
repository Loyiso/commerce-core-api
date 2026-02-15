using CartService.API.Contracts; 

namespace CartService.API.Services;

public interface ILoggingApiClient
{
    Task LogAsync(LoggingApiRequest request, CancellationToken ct = default);
}
