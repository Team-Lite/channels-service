namespace ChannelsService.Application.GetChannel;

public sealed record GetChannelQuery(Guid ChannelId) : Query;