namespace ChannelsService.Application.GetUserChannels;

public sealed record GetUserChannelsQuery(Guid UserId) : Query;