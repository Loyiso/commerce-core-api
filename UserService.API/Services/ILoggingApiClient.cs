using UserService.API.Contracts;
namespace UserService.API.Services;

public interface ILoggingApiClient
{
    Task LogAsync(LoggingApiRequest request, CancellationToken ct = default);
}
