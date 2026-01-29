namespace ChannelsService.Core.Outbox.Payloads;

public class ChannelUserPayload
{
    public Guid UserId { get; init; }
    public Guid ChannelId { get; init; }
}