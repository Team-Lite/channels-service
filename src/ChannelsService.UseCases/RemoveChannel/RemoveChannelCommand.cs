namespace ChannelsService.UseCases.RemoveChannel;

public sealed record RemoveChannelCommand(Guid ChannelId, Guid InitiatorId) : Command;