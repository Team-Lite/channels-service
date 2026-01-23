namespace ChannelsService.UseCases.CreateChannel;

public sealed record CreateChannelCommand(Guid CreatorId, string Name, string? Description) : Command; 