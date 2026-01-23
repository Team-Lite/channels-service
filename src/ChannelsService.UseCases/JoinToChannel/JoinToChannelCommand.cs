namespace ChannelsService.UseCases.JoinToChannel;

public sealed record JoinToChannelCommand(Guid ChannelId, Guid UserId) : Command;