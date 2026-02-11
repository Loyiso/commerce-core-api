using Shared.Logging.Entities;

namespace Shared.Logging.Dispatch;

public interface ILogDispatcher
{
    bool TryEnqueue(LogEntryEntity entry);
}
