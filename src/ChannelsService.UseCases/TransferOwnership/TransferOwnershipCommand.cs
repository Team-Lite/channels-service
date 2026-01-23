namespace ChannelsService.UseCases.TransferOwnership;

public sealed record TransferOwnershipCommand(Guid ChannelId, Guid InitiatorId, Guid NewOwnerId) : Command;