namespace ChannelsService.Commands.JoinToChannel;

public sealed record JoinToChannelCommand(Guid ChannelId, Guid UserId) : Command;