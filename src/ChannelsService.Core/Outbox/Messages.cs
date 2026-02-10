using ChannelsService.Core.Outbox.Payloads;

namespace ChannelsService.Core.Outbox;

public sealed partial class OutboxMessage 
{
    public static OutboxMessage CreateUserJoinedMessage(ChannelUserPayload payload) => 
        Create(Discriminators.UserJoined, payload);
    
    public static OutboxMessage CreateUserLeftMessage(ChannelUserPayload payload) =>
        Create(Discriminators.UserLeft, payload);
    
    public static OutboxMessage CreateChannelRemovedMessage(ChannelPayload payload) =>
        Create(Discriminators.ChannelWasRemoved, payload);
}