namespace ChannelsService.Application.TransferOwnership;

public sealed record TransferOwnershipCommand(Guid ChannelId, Guid InitiatorId, Guid NewOwnerId) : Command;