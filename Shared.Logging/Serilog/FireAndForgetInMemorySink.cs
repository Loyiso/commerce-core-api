using System.Text.Json;
using Serilog.Core;
using Serilog.Events;
using Shared.Logging.Dispatch;
using Shared.Logging.Entities;

namespace Shared.Logging.Serilog;

public sealed class FireAndForgetInMemorySink : ILogEventSink
{
    private readonly ILogDispatcher _dispatcher;
    private readonly string _service;
    private readonly string _environment;

    public FireAndForgetInMemorySink(ILogDispatcher dispatcher, string service, string environment)
    {
        _dispatcher = dispatcher;
        _service = service;
        _environment = environment;
    }

    public void Emit(LogEvent logEvent)
    {
        try
        {
            var props = logEvent.Properties.ToDictionary(k => k.Key, v => (object?)v.Value.ToString());
            var json = JsonSerializer.Serialize(props);

            var entry = new LogEntryEntity
            {
                Timestamp = logEvent.Timestamp,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                Exception = logEvent.Exception?.ToString(),
                Service = _service,
                Environment = _environment,
                PropertiesJson = json
            };

            _dispatcher.TryEnqueue(entry);
        }
        catch
        {
            // never block the application
        }
    }
}
