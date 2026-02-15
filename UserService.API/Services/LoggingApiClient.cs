using System.Net.Http.Json;
using UserService.API.Contracts;

namespace UserService.API.Services;

public sealed class LoggingApiClient : ILoggingApiClient
{
    private readonly HttpClient _http;

    public LoggingApiClient(HttpClient http) => _http = http;

    public async Task LogAsync(LoggingApiRequest request, CancellationToken ct = default)
    { 
        using var res = await _http.PostAsJsonAsync("/api/logs", request, ct); 
    }
}
