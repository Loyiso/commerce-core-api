using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Shared.Logging.Entities;
using Shared.Logging.Store;

namespace Shared.Logging.Dispatch;

public sealed class ChannelLogDispatcher : BackgroundService, ILogDispatcher
{
    private readonly Channel<LogEntryEntity> _channel;
    private readonly ILogStore _store;

    public ChannelLogDispatcher(ILogStore store)
    {
        _store = store;

        _channel = Channel.CreateBounded<LogEntryEntity>(new BoundedChannelOptions(5000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropWrite
        });
    }

    public bool TryEnqueue(LogEntryEntity entry) => _channel.Writer.TryWrite(entry);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var entry in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await _store.AddAsync(entry, stoppingToken);
        }
    }
}
