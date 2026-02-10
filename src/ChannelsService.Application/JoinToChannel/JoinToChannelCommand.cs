namespace ChannelsService.Application.JoinToChannel;

public sealed record JoinToChannelCommand(Guid ChannelId, Guid UserId) : Command;