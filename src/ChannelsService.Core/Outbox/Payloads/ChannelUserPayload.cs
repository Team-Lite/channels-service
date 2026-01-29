namespace ChannelsService.Core.Outbox.Payloads;

public sealed class ChannelUserPayload
{
    public Guid UserId { get; init; }
    public Guid ChannelId { get; init; }
}