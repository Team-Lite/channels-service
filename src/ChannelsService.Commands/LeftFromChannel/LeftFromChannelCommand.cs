namespace ChannelsService.Commands.LeftFromChannel;

public sealed record LeftFromChannelCommand(Guid ChannelId, Guid UserId) : Command;