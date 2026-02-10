namespace ChannelsService.Application.GetUserChannels;

public sealed record UserChannelDto(
    Guid ChannelId,
    string Name);