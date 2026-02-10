namespace ChannelsService.Application.RemoveChannel;

public sealed record RemoveChannelCommand(Guid ChannelId, Guid InitiatorId) : Command;