using ChannelsService.Core.Outbox;
using ChannelsService.External.Database;
using ChannelsService.Outbox.MessageQueue;
using Microsoft.EntityFrameworkCore;

namespace ChannelsService.Outbox.Service;

internal sealed class OutboxService : IOutboxService
{
    private readonly IMessageQueue _messageQueue;
    private readonly DatabaseContext _context;

    public OutboxService(IMessageQueue messageQueue, DatabaseContext context)
    {
        _messageQueue = messageQueue;
        _context = context;
    }

    public async Task PushMessagesAsync(CancellationToken token)
    {
        List<OutboxMessage> messages = await _context
            .OutboxMessages
            .Where(x => !x.IsCompleted)
            .ToListAsync(token);

        foreach (var message in messages)
        {
            await _messageQueue.PushMessageAsync(message);
            message.MarkAsCompleted();
        }

        await _context.SaveChangesAsync(token);
    }
}