using System.Net.Http.Json; 
using IdentityService.API.Models;

namespace IdentityService.API.Services;

public sealed class LoggingApiClient : ILoggingApiClient
{
    private readonly HttpClient _http;

    public LoggingApiClient(HttpClient http) => _http = http;

    public async Task LogAsync(LoggingApiRequest request, CancellationToken ct = default)
    { 
        using var res = await _http.PostAsJsonAsync("/api/logs", request, ct); 
    }
}
