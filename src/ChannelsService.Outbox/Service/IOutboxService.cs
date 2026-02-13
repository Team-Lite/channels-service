using ChannelsService.Core.Outbox;

namespace ChannelsService.Outbox.Service;

internal interface IOutboxService
{
    Task PushMessagesAsync(CancellationToken token);
}