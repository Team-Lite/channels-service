namespace ChannelsService.Commands.RemoveChannel;

public sealed record RemoveChannelCommand(Guid ChannelId, Guid InitiatorId) : Command;