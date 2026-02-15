using System.Net.Http.Json;   
using CatalogService.API.Application.Interfaces; 
using CatalogService.API.Application.Common;  

namespace CatalogService.API.Application.Services;

public sealed class LoggingApiClient : ILoggingApiClient
{
    private readonly HttpClient _http;

    public LoggingApiClient(HttpClient http) => _http = http;

    public async Task LogAsync(LoggingApiRequest request, CancellationToken ct = default)
    { 
        using var res = await _http.PostAsJsonAsync("/api/logs", request, ct); 
    }
}
