namespace ChannelsService.Application.LeftFromChannel;

public sealed record LeftFromChannelCommand(Guid ChannelId, Guid UserId) : Command;