namespace ChannelsService.Application.GetChannel;

public sealed record ChannelDto(Guid ChannelId, string Name, string Description, IEnumerable<ChannelUserDto> Users);

public sealed record ChannelUserDto(Guid UserId, bool IsOwner);