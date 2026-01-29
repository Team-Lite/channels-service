namespace ChannelsService.Commands.CreateChannel;

public sealed record CreateChannelCommand(Guid CreatorId, string Name, string? Description) : Command; 