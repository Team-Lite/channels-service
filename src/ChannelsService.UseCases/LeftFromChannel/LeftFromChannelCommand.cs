namespace ChannelsService.UseCases.LeftFromChannel;

public sealed record LeftFromChannelCommand(Guid ChannelId, Guid UserId) : Command;