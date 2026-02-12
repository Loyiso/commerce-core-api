using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Logging.Data;
using Shared.Logging.Dispatch;
using Shared.Logging.Store;

namespace Shared.Logging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInMemoryLogging(this IServiceCollection services, string? databaseName = null)
    {
        databaseName ??= $"LogsDb_{Guid.NewGuid():N}";

        // Use factory so singletons can create DbContexts safely when needed
        services.AddDbContextFactory<LoggingDbContext>(o => o.UseInMemoryDatabase(databaseName));

        // Make store singleton (so it can be used by singleton dispatcher)
        services.AddSingleton<ILogStore, InMemoryLogStore>();

        // Dispatcher is singleton + hosted service
        services.AddSingleton<ILogDispatcher, ChannelLogDispatcher>();
        services.AddHostedService(sp => (ChannelLogDispatcher)sp.GetRequiredService<ILogDispatcher>());

        return services;
    }
}
