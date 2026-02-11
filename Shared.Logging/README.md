# Shared.Logging

Shared logging library that provides:
- EF Core **InMemory** `LoggingDbContext` for log persistence (per-process)
- Background `ChannelLogDispatcher` for async (fire-and-forget) persistence
- Serilog sink `FireAndForgetInMemorySink`

## Install
Reference this project from your services:

```bash
dotnet add <YourService>.csproj reference ../Shared.Logging/Shared.Logging.csproj
```

## Wire up in Program.cs

```csharp
using Serilog;
using Shared.Logging.Dispatch;
using Shared.Logging.Extensions;
using Shared.Logging.Serilog;

builder.Services.AddSharedInMemoryLogging(databaseName: "LogsDb");

var app = builder.Build();

var dispatcher = app.Services.GetRequiredService<ILogDispatcher>();
var serviceName = builder.Configuration["Serilog:Properties:Application"] ?? app.Environment.ApplicationName;
var envName = app.Environment.EnvironmentName;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // per-service sinks (file/console)
    .Enrich.FromLogContext()
    .WriteTo.Sink(new FireAndForgetInMemorySink(dispatcher, serviceName, envName))
    .CreateLogger();
```

## Notes
EF Core InMemory is **per service process**. To centralize logs across services, create a LoggingService.API or use a shared store (SQL/Elastic/etc.).
