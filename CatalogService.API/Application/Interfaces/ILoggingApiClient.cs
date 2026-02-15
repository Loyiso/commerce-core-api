using CatalogService.API.Application.Services; 
using CatalogService.API.Infrastructure;
using CatalogService.API.Application.Common; 

namespace CatalogService.API.Application.Interfaces;

public interface ILoggingApiClient
{
    Task LogAsync(LoggingApiRequest request, CancellationToken ct = default);
}
