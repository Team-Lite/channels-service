using ChannelsService.Core.Outbox;

namespace ChannelsService.Outbox.MessageQueue;

internal interface IMessageQueue: IAsyncDisposable
{
    Task PushMessageAsync(OutboxMessage message);
}